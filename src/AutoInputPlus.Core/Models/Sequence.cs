using AutoInputPlus.Core.Constants;

namespace AutoInputPlus.Core.Models;

/// <summary>
/// Represents an ordered collection of input steps that are
/// executed sequentially as part of a profile.
/// </summary>
public sealed class Sequence
{
    /// <summary>
    /// Gets or sets the display name of the sequence.
    /// </summary>
    public string Name { get; set; } = AppConstants.DefaultSequenceName;

    /// <summary>
    /// Gets or sets a value indicating whether the sequence is enabled.
    /// Disabled sequences are ignored during execution.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the ordered collection of steps that make up the sequence.
    /// </summary>
    public List<SequenceStep> Steps { get; set; } = [];
}