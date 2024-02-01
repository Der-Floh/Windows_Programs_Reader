﻿using Windows_Programs_Reader.Data;
using Windows_Programs_Reader.Service.EmbeddedResource;

namespace Windows_Programs_Reader.Service.WindowsLanguage;

/// <summary>
/// Service for managing Windows language data.
/// </summary>
public sealed class WindowsLanguageService : IWindowsLanguageService
{
    private const string LanguageListName = "languages.csv";
    private const char SplitChar = ';';

    private readonly IEmbeddedResourceService _embeddedResourceService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsLanguageService"/>.
    /// </summary>
    /// <param name="embeddedResourceService">The embedded resource service.</param>
    public WindowsLanguageService(IEmbeddedResourceService embeddedResourceService) => _embeddedResourceService = embeddedResourceService;

    public IEnumerable<WindowsLanguageData>? GetAll()
    {
        var languages = new List<WindowsLanguageData>();
        using var reader = new StreamReader(_embeddedResourceService.GetResourceStream(LanguageListName));
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            var parts = line.Split(SplitChar);
            var language = new WindowsLanguageData
            {
                Name = parts[0],
                Code = parts[1],
                LCIDCode = parts[2],
                WindowsCodeDecimal = uint.TryParse(parts[3], out var result3) ? result3 : 0,
                WindowsCodeHex = parts[4],
                CodePage = uint.TryParse(parts[5], out var result5) ? result5 : 0
            };

            languages.Add(language);
        }
        return languages;
    }
}
