using System.Drawing;
using Windows_Programs_Reader.Extensions;
using WindowsIconExtention;

namespace Windows_Programs_Reader.Utility;

/// <summary>
/// Handles loading of icons for applications from various sources.
/// </summary>
public sealed class IconLoader
{
    public string? Id { get; set; }
    public string? DisplayIcon { get; set; }
    public string? InstallLocation { get; set; }
    public string? DisplayName { get; set; }

    /// <summary>
    /// Initializes a new instance of the<see cref="IconLoader"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the application (optional).</param>
    /// <param name="displayIcon">The path or file name of the application's display icon (optional).</param>
    /// <param name="installLocation">The installation location of the application (optional).</param>
    /// <param name="displayName">The display name of the application (optional).</param>
    public IconLoader(string? id = null, string? displayIcon = null, string? installLocation = null, string? displayName = null)
    {
        Id = id;
        DisplayIcon = displayIcon;
        InstallLocation = installLocation;
        DisplayName = displayName;
    }

    /// <summary>
    /// Retrieves the icon for the application based on its properties.
    /// </summary>
    /// <returns>A <see cref="Bitmap"/> object representing the icon, or null if no icon could be found.</returns>
    public Bitmap? GetIcon()
    {
        Bitmap? icon = null;
        if (!string.IsNullOrEmpty(DisplayIcon))
            icon = GetIconFromDisplayIconPath(DisplayIcon);
        if (icon is null && !string.IsNullOrEmpty(DisplayName))
        {
            if (!string.IsNullOrEmpty(Id) && Id.StartsWith('{') && Id.EndsWith('}'))
                icon = GetIconFromWindowsInstallerCache(Id, DisplayName);
            if (icon is null && !string.IsNullOrEmpty(InstallLocation))
                icon = GetIconFromAppDirectory(InstallLocation, DisplayName);
        }

        return icon;
    }

    private static Bitmap? GetIconFromDisplayIconPath(string displayIcon)
    {
        Bitmap? icon = null;
        var extension = Path.GetExtension(displayIcon).ToLower();
        if (extension.Contains(".ico") || extension.Contains(".exe"))
        {
            var iconPath = RemoveIconIndex(displayIcon);
            iconPath = iconPath.Trim('"');
            if (File.Exists(iconPath))
                icon = GetIconFromFile(iconPath);
        }
        else if (extension.Contains(".jpg") || extension.Contains(".jpeg") || extension.Contains(".png"))
        {
            if (File.Exists(displayIcon))
                icon = new Bitmap(displayIcon);
        }
        return icon;
    }

    private static Bitmap? GetIconFromAppDirectory(string installLocation, string displayName)
    {
        IEnumerable<string> files = Directory.EnumerateFiles(installLocation, "*.exe", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (Path.GetFileNameWithoutExtension(file).ContainsGeneralized(displayName))
            {
                var iconData = new IconData(file);
                var icon = iconData.GetIconWithBestQuality().ToBitmap();
                return icon;
            }
        }
        return null;
    }

    private static Bitmap? GetIconFromWindowsInstallerCache(string guid, string displayName)
    {
        var windowsPath = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.System)) ?? string.Empty;
        var installerPath = Path.Combine(windowsPath, "Installer", guid);
        if (!Directory.Exists(installerPath))
            return null;

        var iconPath = GetIconPathFromDirectoryIco(installerPath, displayName);
        iconPath ??= GetIconPathFromDirectoryExe(installerPath, displayName);
        iconPath ??= GetIconPathFromDirectoryNoExt(installerPath, displayName);

        if (File.Exists(iconPath))
        {
            string? extension = null;
            if (string.IsNullOrEmpty(Path.GetExtension(iconPath)))
                extension = ".ico";
            try
            {
                var iconData = new IconData(iconPath, extension);
                var icon = iconData.GetIconWithBestQuality().ToBitmap();
                return icon;
            }
            catch { return null; }
        }
        else
        {
            return null;
        }
    }

    private static string? GetIconPathFromDirectoryIco(string directoryPath, string displayName)
    {
        FileInfo[] icoFiles = new DirectoryInfo(directoryPath).GetFiles("*.ico").OrderByDescending(x => x.Length).ToArray();
        return GetIconFileFromFiles(icoFiles, displayName);
    }

    private static string? GetIconPathFromDirectoryExe(string directoryPath, string displayName)
    {
        FileInfo[] exeFiles = new DirectoryInfo(directoryPath).GetFiles("*.exe").OrderByDescending(x => x.Length).ToArray();
        return GetIconFileFromFiles(exeFiles, displayName);
    }

    private static string? GetIconPathFromDirectoryNoExt(string directoryPath, string displayName)
    {
        FileInfo[] allFiles = new DirectoryInfo(directoryPath).GetFiles();
        FileInfo[] filesWithoutExt = allFiles.Where(x => string.IsNullOrEmpty(x.Extension)).ToArray();
        return GetIconFileFromFiles(filesWithoutExt, displayName);
    }

    private static string? GetIconFileFromFiles(FileInfo[] files, string displayName)
    {
        if (files.Length != 0)
        {
            FileInfo? file = FindSimilarDisplayName(files, displayName);
            file ??= files[0];
            return file.FullName;
        }
        return null;
    }

    /// <summary>
    /// Finds a file with a display name similar to the specified display name.
    /// </summary>
    /// <param name="files">An array of <see cref="FileInfo"/> objects to search through.</param>
    /// <param name="displayName">The display name to find a similar match for.</param>
    /// <returns>A <see cref="FileInfo"/> object that has a similar display name, or null if no match is found.</returns>
    public static FileInfo? FindSimilarDisplayName(FileInfo[] files, string displayName)
    {
        FileInfo? similarFile = null;
        foreach (FileInfo file in files)
        {
            if (Path.GetFileNameWithoutExtension(file.FullName).ContainsGeneralized(displayName))
            {
                similarFile = file;
                break;
            }
        }
        return similarFile;
    }

    /// <summary>
    /// Retrieves an icon from a file.
    /// </summary>
    /// <param name="iconPath">The path to the file containing the icon.</param>
    /// <returns>A <see cref="Bitmap"/> representing the icon extracted from the file, or null if the icon cannot be extracted.</returns>
    public static Bitmap? GetIconFromFile(string iconPath)
    {
        var iconData = new IconData(iconPath);
        var icon = iconData.GetIconWithBestQuality().ToBitmap();
        return icon;
    }

    /// <summary>
    /// Removes the icon index from a file path, if present.
    /// </summary>
    /// <param name="filePath">The file path that may contain an icon index.</param>
    /// <returns>The file path with the icon index removed, if it was present.</returns>
    public static string RemoveIconIndex(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        if (extension.Contains(','))
            extension = extension[..extension.LastIndexOf(',')];
        var extensionIndex = filePath.LastIndexOf(extension) + extension.Length - 1;
        var index = filePath.LastIndexOf(',');
        return index < extensionIndex ? filePath : filePath[..index];
    }
}
