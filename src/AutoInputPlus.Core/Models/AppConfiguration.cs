using System.Diagnostics.CodeAnalysis;

namespace AutoInputPlus.Core.Models;

/// <summary>
/// Represents global application configuration loaded during startup.
/// These values control application-wide behavior and are not part of
/// any shareable input profile.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class AppConfiguration
{
    /// <summary>
    /// Gets or sets the default folder path used to store application data.
    /// </summary>
    public string DataFolderPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the last active profile.
    /// </summary>
    public Guid? LastActiveProfileId { get; set; }

    /// <summary>
    /// Flag that indicates if the application should run when Operating System starts.
    /// </summary>
    public bool RunOnSystemStartup { get; set; } //TODO Stored in a registry key? Maybe this is not needed, but instead the UI modifies the registry? more research on this...
}