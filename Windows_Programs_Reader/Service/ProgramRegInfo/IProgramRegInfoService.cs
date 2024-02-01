using System.Drawing;
using Windows_Programs_Reader.Data;

namespace Windows_Programs_Reader.Service.ProgramRegInfo;

public interface IProgramRegInfoService
{
    /// <summary>
    /// Fetches fallback values for the program.
    /// </summary>
    void FetchFallbackValues(ProgramRegInfoData programRegInfoData);

    /// <summary>
    /// Gets the icon associated with the program.
    /// </summary>
    /// <returns>A Bitmap that represents the Icon.</returns>
    Bitmap? GetIcon(ProgramRegInfoData programRegInfoData);
}
