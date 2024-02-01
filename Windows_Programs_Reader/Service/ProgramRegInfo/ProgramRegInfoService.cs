using System.Drawing;
using Windows_Programs_Reader.Data;
using Windows_Programs_Reader.Utility;

namespace Windows_Programs_Reader.Service.ProgramRegInfo;

public sealed class ProgramRegInfoService : IProgramRegInfoService
{
    public void FetchFallbackValues(ProgramRegInfoData programRegInfoData)
    {
        if (string.IsNullOrEmpty(programRegInfoData.InstallLocation) && !string.IsNullOrEmpty(programRegInfoData.DisplayIcon))
        {
            var iconPath = IconLoader.RemoveIconIndex(programRegInfoData.DisplayIcon);
            iconPath = iconPath.Trim('"');
            var extension = Path.GetExtension(programRegInfoData.DisplayIcon).ToLower();
            if (extension.Contains(".exe"))
                programRegInfoData.InstallLocation = Path.GetDirectoryName(iconPath) ?? string.Empty;
        }

        if ((string.IsNullOrEmpty(programRegInfoData.EstimatedSize) || programRegInfoData.EstimatedSize == "0") && !string.IsNullOrEmpty(programRegInfoData.InstallLocation))
        {
            try
            {
                var directoryInfo = new DirectoryInfo(programRegInfoData.InstallLocation);
                long totalSize = 0;
                FileInfo[] fileInfos = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo fileInfo in fileInfos)
                    totalSize += fileInfo.Length;
                programRegInfoData.EstimatedSize = (totalSize / 1024).ToString();
            }
            catch { }
        }
    }

    public Bitmap? GetIcon(ProgramRegInfoData programRegInfoData)
    {
        try
        {
            var iconLoader = new IconLoader(programRegInfoData.Id, programRegInfoData.DisplayIcon, programRegInfoData.InstallLocation, programRegInfoData.DisplayName);
            return iconLoader.GetIcon();
        }
        catch { return null; }
    }
}
