using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Infrastructure.Persistence;
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
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        //Persistence services
        services.AddSingleton<IAppConfigurationStore, AppConfigurationStore>();
        services.AddSingleton<IInputProfileStore, InputProfileStore>();

        return services;
    }
}