using AutoInputPlus.Core.Constants;

namespace AutoInputPlus.Core.Models;

/// <summary>
/// Represents a shareable input automation profile.
/// A profile defines user-configurable automation behavior,
/// including sequences, timing options, and execution settings.
/// </summary>
public sealed class InputProfile
{
    /// <summary>
    /// Gets or sets the unique identifier of the profile.
    /// This value is used internally to distinguish profiles,
    /// even when multiple profiles share the same display name.
    /// </summary>
    public Guid ProfileId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the display name of the profile.
    /// This value is intended for user-facing display and may not be unique.
    /// </summary>
    public string Name { get; set; } = AppConstants.DefaultInputProfileName;

    /// <summary>
    /// Gets or sets the collection of sequences defined for the profile.
    /// </summary>
    public List<Sequence> Sequences { get; set; } = [];
}