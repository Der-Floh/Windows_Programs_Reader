using Windows_Programs_Reader.Data;
using Windows_Programs_Reader.Repository.ProgramRegInfo;
using Windows_Programs_Reader.Service.EmbeddedResource;
using Windows_Programs_Reader.Service.ProgramInfo;
using Windows_Programs_Reader.Service.WindowsLanguage;

namespace Windows_Programs_Reader.Repository.ProgramInfo;

/// <summary>
/// Repository for retrieving and processing information about installed programs.
/// </summary>
public sealed class ProgramInfoRepository : IProgramInfoRepository
{
    private readonly IProgramInfoService _programInfoService;
    private readonly IProgramRegInfoRepository _programRegInfoRepository;
    private readonly IWindowsLanguageService _windowsLanguageService;

    private readonly IEnumerable<WindowsLanguageData>? _languages;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgramInfoRepository"/> class with specified services.
    /// </summary>
    /// <param name="programInfoService">Service for handling program information.</param>
    /// <param name="programRegInfoRepository">Repository for accessing registry information about programs.</param>
    /// <param name="windowsLanguageService">Service for handling Windows language data.</param>
    public ProgramInfoRepository(IProgramInfoService programInfoService, IProgramRegInfoRepository programRegInfoRepository, IWindowsLanguageService windowsLanguageService)
    {
        _programInfoService = programInfoService;
        _programRegInfoRepository = programRegInfoRepository;
        _windowsLanguageService = windowsLanguageService;

        _languages = _windowsLanguageService.GetAll();
    }

    /// <summary>
    /// Default constructor that initializes the repository with default services.
    /// </summary>
    public ProgramInfoRepository()
    {
        _programInfoService = new ProgramInfoService();
        _programRegInfoRepository = new ProgramRegInfoRepository();
        var embeddedResourceService = new EmbeddedResourceService();
        _windowsLanguageService = new WindowsLanguageService(embeddedResourceService);

        _languages = _windowsLanguageService.GetAll();
    }

    /// <summary>
    /// Retrieves all program information and optionally performs an action on each item.
    /// </summary>
    /// <param name="action">An optional action to perform on each <see cref="ProgramInfoData"/> item.</param>
    /// <returns>An enumerable collection of <see cref="ProgramInfoData"/>.</returns>
    public IEnumerable<ProgramInfoData> GetAll(Action<ProgramInfoData>? action = null)
    {
        IEnumerable<ProgramRegInfoData> programRegInfos = _programRegInfoRepository.GetAll();

        var programInfos = new List<ProgramInfoData>();
        foreach (ProgramRegInfoData program in programRegInfos)
        {
            try
            {
                program.FetchFallbackValues();
                ProgramInfoData programInfo = FromProgramRegInfo(program);
                action?.Invoke(programInfo);
                programInfos.Add(programInfo);
            }
            catch { }
        }

        return programInfos;
    }

    /// <summary>
    /// Converts program registry information to <see cref="ProgramInfoData"/>.
    /// </summary>
    /// <param name="regInfo">The registry information of a program.</param>
    /// <returns>A <see cref="ProgramInfoData"/> object populated with data from the registry.</returns>
    private ProgramInfoData FromProgramRegInfo(ProgramRegInfoData regInfo)
    {
        var programInfo = new ProgramInfoData(_programInfoService)
        {
            RegKey = regInfo.RegKey,

            Id = regInfo.Id,
            Comments = regInfo.Comments,
            Contact = regInfo.Contact,

            DisplayIcon = regInfo.GetIcon(),
            DisplayName = regInfo.DisplayName,
            DisplayVersion = regInfo.DisplayVersion,

            HelpLink = regInfo.HelpLink,
            HelpTelephone = regInfo.HelpTelephone,
            InstallDate = regInfo.InstallDate,
            InstallLocation = string.IsNullOrEmpty(regInfo.InstallDir) ? regInfo.InstallLocation : regInfo.InstallDir,
            InstallSource = regInfo.InstallSource,

            ModifyPath = regInfo.ModifyPath,
            NoModify = regInfo.NoModify == "1",
            NoRemove = regInfo.NoRemove == "1",
            NoRepair = regInfo.NoRepair == "1",

            Publisher = regInfo.Publisher,
            Readme = regInfo.Readme,
            SystemComponent = regInfo.SystemComponent == "1",

            QuietUninstallString = regInfo.QuietUninstallString,
            UninstallString = regInfo.UninstallString,
            UrlInfoAbout = regInfo.UrlInfoAbout,
            UrlUpdateInfo = regInfo.UrlUpdateInfo,

            WindowsInstaller = regInfo.WindowsInstaller == "1",
        };

        if (!string.IsNullOrEmpty(regInfo.EstimatedSize) && long.TryParse(regInfo.EstimatedSize, out var size))
            programInfo.EstimatedSize = size * 1024;

        if (!string.IsNullOrEmpty(regInfo.Language))
        {
            WindowsLanguageData? language = _languages?.FirstOrDefault(x => x.WindowsCodeDecimal.ToString() == regInfo.Language);
            programInfo.Language = language?.LCIDCode;
        }

        if (!string.IsNullOrEmpty(regInfo.VersionMajor) && long.TryParse(regInfo.VersionMajor, out var versionMajor))
            programInfo.VersionMajor = versionMajor;

        if (!string.IsNullOrEmpty(regInfo.VersionMinor) && long.TryParse(regInfo.VersionMinor, out var versionMinor))
            programInfo.VersionMinor = versionMinor;

        return programInfo;
    }
}
