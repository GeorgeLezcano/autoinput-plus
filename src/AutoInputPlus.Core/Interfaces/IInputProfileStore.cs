using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Core.Interfaces;

/// <summary>
/// Defines persistence operations for input profiles.
/// </summary>
public interface IInputProfileStore
{
    /// <summary>
    /// Loads a profile by its unique identifier.
    /// </summary>
    /// <param name="profileId">
    /// The unique identifier of the profile to load.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous load operation.
    /// The task result contains the loaded input profile.
    /// </returns>
    Task<InputProfile> LoadProfile(Guid profileId);

    /// <summary>
    /// Saves an input profile.
    /// </summary>
    /// <param name="profile">
    /// The profile to save.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous save operation.
    /// </returns>
    Task SaveProfile(InputProfile profile);

    /// <summary>
    /// Deletes a profile by its unique identifier.
    /// </summary>
    /// <param name="profileId">
    /// The unique identifier of the profile to delete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous delete operation.
    /// </returns>
    Task DeleteProfile(Guid profileId);

    /// <summary>
    /// Gets all available profiles.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous retrieval operation.
    /// The task result contains a read-only collection of available input profiles.
    /// </returns>
    Task<IReadOnlyList<InputProfile>> GetAll();

    /// <summary>
    /// Determines whether a profile with the specified unique identifier exists.
    /// </summary>
    /// <param name="profileId">
    /// The unique identifier of the profile to locate.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous existence check.
    /// The task result is <see langword="true"/> if the profile exists; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    Task<bool> Exists(Guid profileId);
}