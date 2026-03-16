using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Infrastructure.Persistence;

/// <summary>
/// AutoInput implementation for global app configuration.
/// </summary>
public sealed class AppConfigurationStore : IAppConfigurationStore
{
    /// <inheritdoc/>
    public async Task<AppConfiguration> LoadConfigurationAsync()
    {   
        AppConfiguration config = new();

        // TODO Load the configuration at startup from persistence.

        return await Task.FromResult(config);
    }

    /// <inheritdoc/>
    public async Task SaveConfigurationAsync(AppConfiguration configuration)
    {
        //TODO Save the app configuration so it persists through restarts.

        await Task.CompletedTask;
    }
}