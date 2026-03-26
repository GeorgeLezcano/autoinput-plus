using System.Runtime.Versioning;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Infrastructure.Persistence;
using AutoInputPlus.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AutoInputPlus.Infrastructure;

/// <summary>
/// Provides dependency injection registration methods for infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers infrastructure services with the specified service collection.
    /// </summary>
    /// <param name="services">
    /// The service collection to update.
    /// </param>
    /// <returns>
    /// The same service collection instance for chaining.
    /// </returns>
    [SupportedOSPlatform("windows")]
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        //Persistence services
        services.AddSingleton<IAppConfigurationStore, AppConfigurationStore>();
        services.AddSingleton<IInputProfileStore, InputProfileStore>();

        // Platform services
        services.AddSingleton<IStartupRegistrationService>(_ =>
            new StartupRegistrationService(Environment.ProcessPath
                ?? throw new InvalidOperationException("Unable to determine application executable path.")));

        return services;
    }
}