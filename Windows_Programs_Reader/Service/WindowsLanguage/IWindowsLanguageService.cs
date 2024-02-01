using Windows_Programs_Reader.Data;

namespace Windows_Programs_Reader.Service.WindowsLanguage;

public interface IWindowsLanguageService
{
    /// <summary>
    /// Retrieves all language data.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="WindowsLanguageData"/>, or null if the data cannot be loaded.</returns>
    IEnumerable<WindowsLanguageData>? GetAll();
}
