using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Engine.Profile;

/// <summary>
/// Autoinput implementation to manage the
/// active profile.
/// </summary>
public sealed class ProfileManager : IProfileManager
{
    /// <inheritdoc/>
    public InputProfile? ActiveProfile => throw new NotImplementedException();

    /// <inheritdoc/>
    public void ClearActiveProfile()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetActiveProfile(InputProfile profile)
    {
        throw new NotImplementedException();
    }
}