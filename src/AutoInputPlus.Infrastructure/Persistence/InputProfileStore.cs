using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Infrastructure.Persistence;

/// <summary>
/// AutoInput implementation for Input profiles.
/// It handles persistence and retrieval.
/// </summary>
public sealed class InputProfileStore : IInputProfileStore
{
    /// <inheritdoc/>
    public Task DeleteProfileAsync(Guid profileId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(Guid profileId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<InputProfile>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<InputProfile> LoadProfileAsync(Guid profileId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SaveProfileAsync(InputProfile profile)
    {
        throw new NotImplementedException();
    }
}