using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kentico.Xperience.Mjml.StarterKit.Rcl;

/// <summary>
/// Application startup extension methods.
/// </summary>
public static class MjmlStarterKitStartupExtensions
{
    /// <summary>
    /// Adds MJML starter kit services to application with customized options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> which will be modified.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> where <see cref="MjmlStarterKitOptions"/> are specified.</param>
    /// <returns>This instance of <see cref="IServiceCollection"/>, allowing for further configuration in a fluent manner.</returns>
    public static IServiceCollection AddKenticoMjmlStarterKit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CssLoaderService>()
            .Configure<MjmlStarterKitOptions>(configuration.GetSection(nameof(MjmlStarterKitOptions)));

        return services;
    }

    /// <summary>
    /// Adds MJML starter kit services to application with customized options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> which will be modified.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="MjmlStarterKitOptions"/>.</param>
    /// <returns>This instance of <see cref="IServiceCollection"/>, allowing for further configuration in a fluent manner.</returns>
    public static IServiceCollection AddKenticoMjmlStarterKit(this IServiceCollection services, Action<MjmlStarterKitOptions> configureOptions)
    {
        services.AddScoped<CssLoaderService>()
            .Configure(configureOptions);

        return services;
    }
}
