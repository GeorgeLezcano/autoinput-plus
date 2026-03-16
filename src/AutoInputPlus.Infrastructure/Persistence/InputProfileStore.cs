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
    public Task DeleteProfile(Guid profileId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<bool> Exists(Guid profileId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<InputProfile>> GetAll()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<InputProfile> LoadProfile(Guid profileId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SaveProfile(InputProfile profile)
    {
        throw new NotImplementedException();
    }
}