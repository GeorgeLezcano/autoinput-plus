namespace AutoInputPlus.Core.Models;

/// <summary>
/// Represents global application configuration loaded during startup.
/// These values control application-wide behavior and are not part of
/// any shareable input profile.
/// </summary>
public class AppConfiguration
{
    /// <summary>
    /// Gets or sets the default folder path used to store application data.
    /// </summary>
    public string DataFolderPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the last active profile.
    /// </summary>
    public Guid? LastActiveProfileId { get; set; }
}