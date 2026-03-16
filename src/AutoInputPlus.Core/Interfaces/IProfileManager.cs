using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Core.Interfaces;

/// <summary>
/// Defines operations for managing the currently active input profile.
/// </summary>
public interface IProfileManager
{
    /// <summary>
    /// Gets the currently active profile, if one has been selected.
    /// Uses a default profile otherwise.
    /// </summary>
    InputProfile ActiveProfile { get; }

    /// <summary>
    /// Sets the active profile.
    /// </summary>
    /// <param name="profile">
    /// The profile to activate.
    /// </param>
    void SetActiveProfile(InputProfile profile);
}