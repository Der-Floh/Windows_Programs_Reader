using System.Diagnostics;
using System.Reflection;
using Windows_Programs_Reader.Data;
using Windows_Programs_Reader.Service.EmbeddedResource;
using Windows_Programs_Reader.Service.RegJump;

namespace Windows_Programs_Reader.Service.ProgramInfo;

/// <summary>
/// Service for managing operations related to program information, such as updating, uninstalling, and modifying installed programs.
/// </summary>
public sealed class ProgramInfoService : IProgramInfoService
{
    private const string CmdFileName = "cmd.exe";
    private const string RunAsAdminVerb = "runas";
    private readonly IRegJumpService _regJumpWrapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgramInfoService"/> class with a specified RegJumpWrapper.
    /// </summary>
    /// <param name="regJumpWrapper">The RegJumpWrapper to use for registry operations.</param>
    public ProgramInfoService(IRegJumpService regJumpWrapper) => _regJumpWrapper = regJumpWrapper;

    /// <summary>
    /// Default constructor that initializes the service with a new instance of RegJumpWrapper.
    /// </summary>
    public ProgramInfoService()
    {
        var _embeddedResourceService = new EmbeddedResourceService();
        _regJumpWrapper = new RegJumpService(_embeddedResourceService);
    }

    public void UpdateFromDifferent(ProgramInfoData programInfoData, ProgramInfoData programInfoDataToCopy)
    {
        foreach (PropertyInfo property in programInfoData.GetType().GetProperties())
        {
            var originalValue = property.GetValue(programInfoData);
            var updateValue = property.GetValue(programInfoDataToCopy);
            if (updateValue is not null && originalValue != updateValue)
                property.SetValue(programInfoData, property.GetValue(programInfoDataToCopy));
        }
    }

    public async Task<bool> Uninstall(ProgramInfoData programInfoData, bool quiet = false)
    {
        var arguments = programInfoData.UninstallString;
        if (quiet)
            arguments = programInfoData.QuietUninstallString;

        return await RunProcess(CmdFileName, arguments);
    }

    public async Task<bool> Modify(ProgramInfoData programInfoData, string? additionalArguments = null)
    {
        var arguments = programInfoData.ModifyPath;
        if (!string.IsNullOrEmpty(additionalArguments))
            arguments += " " + additionalArguments;

        return await RunProcess(CmdFileName, arguments);
    }

    public async Task<bool> OpenRegistry(ProgramInfoData programInfoData) => !string.IsNullOrEmpty(programInfoData.RegKey) && await _regJumpWrapper.OpenAt(programInfoData.RegKey);

    private static async Task<bool> RunProcess(string processName, string? arguments = null)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = processName,
            Arguments = arguments,
            UseShellExecute = true,
            CreateNoWindow = true,
            Verb = RunAsAdminVerb,
        };

        var process = new Process { StartInfo = startInfo };

        try
        {
            process.Start();
            await process.WaitForExitAsync();
            return true;
        }
        catch { return false; }
    }
}
