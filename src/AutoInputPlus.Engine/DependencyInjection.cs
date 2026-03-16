using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Engine.Profile;
using AutoInputPlus.Engine.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace AutoInputPlus.Engine;

/// <summary>
/// Provides dependency injection registration methods for engine services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers engine services with the specified service collection.
    /// </summary>
    /// <param name="services">
    /// The service collection to update.
    /// </param>
    /// <returns>
    /// The same service collection instance for chaining.
    /// </returns>
    public static IServiceCollection AddEngineServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Runtime services
        services.AddSingleton<IEngine, AutoInputEngine>();
        services.AddSingleton<ISequenceRunner, SequenceRunner>();

        // Profile services
        services.AddSingleton<IProfileExchange, ProfileExchange>();
        services.AddSingleton<IProfileManager, ProfileManager>();

        return services;
    }
}