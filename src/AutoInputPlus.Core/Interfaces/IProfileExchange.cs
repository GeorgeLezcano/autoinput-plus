using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Core.Interfaces;

/// <summary>
/// Provides functionality for importing and exporting input profiles
/// to and from portable shareable formats.
/// </summary>
public interface IProfileExchange
{
    /// <summary>
    /// Exports the specified profile into a shareable encoded string.
    /// </summary>
    /// <param name="profile">
    /// The profile to export.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous export operation.
    /// The task result contains the encoded profile string.
    /// </returns>
    Task<string> ExportProfile(InputProfile profile);

    /// <summary>
    /// Imports a profile from an encoded share string.
    /// </summary>
    /// <param name="encodedProfile">
    /// The encoded profile string.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous import operation.
    /// The task result contains the decoded input profile.
    /// </returns>
    Task<InputProfile> ImportProfile(string encodedProfile);

    /// <summary>
    /// Determines whether the specified encoded string
    /// is a valid profile share code.
    /// </summary>
    /// <param name="encodedProfile">
    /// The encoded profile string to validate.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the string represents a valid share code;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool IsValidProfileString(string encodedProfile);
}