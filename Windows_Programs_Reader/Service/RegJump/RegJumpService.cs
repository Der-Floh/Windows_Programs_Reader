using Microsoft.Win32;
using System.Diagnostics;
using Windows_Programs_Reader.Service.EmbeddedResource;
using Windows_Programs_Reader.Utility;

namespace Windows_Programs_Reader.Service.RegJump;

/// <summary>
/// Provides a wrapper for interacting with the RegJump utility to jump to specific registry keys.
/// </summary>
public sealed class RegJumpService : IRegJumpService
{
    private const string RegJumpName = "regjump.exe";
    private const string RunAsAdminVerb = "runas";
    private const string EulaPath = @"HKEY_CURRENT_USER\SOFTWARE\Sysinternals\Regjump\EulaAccepted";
    private readonly string _regJumpPath;

    private readonly IEmbeddedResourceService _embeddedResourceService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegJumpService"/> class.
    /// </summary>
    /// <param name="embeddedResourceService">The embedded resource service.</param>
    public RegJumpService(IEmbeddedResourceService embeddedResourceService)
    {
        _embeddedResourceService = embeddedResourceService;
        _regJumpPath = _embeddedResourceService.GetResourcePath(RegJumpName);
    }

    public async Task<bool> OpenAt(string regKey)
    {
        AcceptEula();
        var startInfo = new ProcessStartInfo
        {
            FileName = _regJumpPath,
            Arguments = regKey,
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

    public bool AcceptEula(bool accept = true)
    {
        var accepted = RegistryHelper.GetRegKeyValue(EulaPath)?.ToString() == "1";
        return accepted || RegistryHelper.SetKey(EulaPath, accept ? 1 : 0, RegistryValueKind.DWord);
    }
}
