using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Core.Interfaces;

/// <summary>
/// Defines persistence operations for global application configuration.
/// </summary>
public interface IAppConfigurationStore
{
    /// <summary>
    /// Loads the global application configuration.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous load operation.
    /// The task result contains the loaded application configuration.
    /// </returns>
    Task<AppConfiguration> LoadConfigurationAsync();

    /// <summary>
    /// Saves the global application configuration.
    /// </summary>
    /// <param name="configuration">
    /// The application configuration to save.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous save operation.
    /// </returns>
    Task SaveConfigurationAsync(AppConfiguration configuration);
}