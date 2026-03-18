using System.Diagnostics.CodeAnalysis;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Input.Windows.Hotkeys;
using AutoInputPlus.Input.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace AutoInputPlus.Input.Windows;

/// <summary>
/// Provides dependency injection registration methods for Windows input services.
/// </summary>
[ExcludeFromCodeCoverage]
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

        services.AddSingleton<IGlobalHotkey, WindowsHotkeyService>();
        services.AddSingleton<IInputSender, InputSender>();
        
        return services;
    }
}