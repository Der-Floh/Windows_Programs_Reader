using Windows_Programs_Reader.Data;

namespace Windows_Programs_Reader.Repository.ProgramInfo;

public interface IProgramInfoRepository
{
    /// <summary>
    /// Retrieves all program information and optionally performs an action on each item.
    /// </summary>
    /// <param name="action">An optional action to perform on each <see cref="ProgramInfoData"/> item.</param>
    /// <returns>An enumerable collection of <see cref="ProgramInfoData"/>.</returns>
    IEnumerable<ProgramInfoData> GetAll(Action<ProgramInfoData>? action = null);
}
