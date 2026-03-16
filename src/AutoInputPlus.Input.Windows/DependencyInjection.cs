using Microsoft.Extensions.DependencyInjection;

namespace AutoInputPlus.Input.Windows;

/// <summary>
/// Provides dependency injection registration methods for Windows input services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers Windows input services with the specified service collection.
    /// </summary>
    /// <param name="services">
    /// The service collection to update.
    /// </param>
    /// <returns>
    /// The same service collection instance for chaining.
    /// </returns>
    public static IServiceCollection AddWindowsInputServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // TODO Serviceregistrations
        
        return services;
    }
}