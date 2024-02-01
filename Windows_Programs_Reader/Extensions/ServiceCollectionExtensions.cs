using Microsoft.Extensions.DependencyInjection;
using Windows_Programs_Reader.Repository.ProgramInfo;
using Windows_Programs_Reader.Repository.ProgramRegInfo;
using Windows_Programs_Reader.Service.EmbeddedResource;
using Windows_Programs_Reader.Service.ProgramInfo;
using Windows_Programs_Reader.Service.ProgramRegInfo;
using Windows_Programs_Reader.Service.RegJump;
using Windows_Programs_Reader.Service.WindowsLanguage;

namespace Windows_Programs_Reader.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services related to Windows program information and Windows language data to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <remarks>
    /// This method registers the following services:<br/>
    /// - <see cref="IProgramInfoRepository"/> implemented by <see cref="ProgramInfoRepository"/>.<br/>
    /// - <see cref="IProgramInfoService"/> implemented by <see cref="ProgramInfoService"/>.<br/>
    /// - <see cref="IProgramRegInfoService"/> implemented by <see cref="ProgramRegInfoService"/>.<br/>
    /// - <see cref="IProgramRegInfoRepository"/> implemented by <see cref="ProgramRegInfoRepository"/>.<br/>
    /// - <see cref="IWindowsLanguageService"/> implemented by <see cref="WindowsLanguageService"/>.<br/>
    /// - <see cref="IRegJumpService"/> implemented by <see cref="RegJumpService"/>.<br/>
    /// - <see cref="IEmbeddedResourceService"/> implemented by <see cref="EmbeddedResourceService"/>.<br/>
    /// These services are essential for managing and accessing information about installed Windows programs and language settings.
    /// </remarks>
    public static IServiceCollection AddWindowsProgramsReader(this IServiceCollection services)
    {
        services.AddSingleton<IProgramInfoRepository, ProgramInfoRepository>();
        services.AddSingleton<IProgramInfoService, ProgramInfoService>();
        services.AddSingleton<IProgramRegInfoService, ProgramRegInfoService>();
        services.AddSingleton<IProgramRegInfoRepository, ProgramRegInfoRepository>();
        services.AddSingleton<IWindowsLanguageService, WindowsLanguageService>();
        services.AddSingleton<IRegJumpService, RegJumpService>();
        services.AddSingleton<IEmbeddedResourceService, EmbeddedResourceService>();
        return services;
    }
}
