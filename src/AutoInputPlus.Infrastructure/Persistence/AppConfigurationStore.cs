using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Infrastructure.Persistence;

/// <summary>
/// AutoInput implementation for global app configuration.
/// </summary>
public sealed class AppConfigurationStore : IAppConfigurationStore
{
    /// <inheritdoc/>
    public Task<AppConfiguration> LoadConfiguration()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SaveConfiguration(AppConfiguration configuration)
    {
        throw new NotImplementedException();
    }
}