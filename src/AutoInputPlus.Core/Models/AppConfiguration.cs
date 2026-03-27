using System.Diagnostics.CodeAnalysis;
using AutoInputPlus.Core.Enums;

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
    /// Gets or sets the unique identifier of the last active profile.
    /// </summary>
    public Guid? LastActiveProfileId { get; set; }

    /// <summary>
    /// Selected application theme.
    /// </summary>
    public AppTheme AppTheme { get; set; }

    /// <summary>
    /// Flag to indicate if the engine should be enabled.
    /// </summary>
    public bool EngineEnabled { get; set; }
}