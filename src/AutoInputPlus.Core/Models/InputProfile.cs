using System.Diagnostics.CodeAnalysis;
using AutoInputPlus.Core.Constants;

namespace AutoInputPlus.Core.Models;

/// <summary>
/// Represents a shareable input automation profile.
/// A profile defines user-configurable automation behavior,
/// including sequences, timing options, and execution settings.
/// </summary>
[ExcludeFromCodeCoverage]
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

    /// <summary>
    /// Interval in milliseconds between inputs.
    /// </summary>
    public int IntervalMilliseconds { get; set; }

    /// <summary>
    /// Flag to indicate if app runs until stopped.
    /// </summary>
    public bool RunUntilStopActive { get; set; }

    /// <summary>
    /// Flag to indicate if app runs until set count. 
    /// </summary>
    public bool RunUntilSetCountActive { get; set; }

    /// <summary>
    /// Input Count until App stops the inputs.
    /// </summary>
    public int StopInputCount { get; set; }

    /// <summary>
    /// Flag that indicates if the hold key is enabled.
    /// </summary>
    public bool HoldTargetEnabled { get; set; }

    /// <summary>
    /// Start/Stop Toggle App keybind.
    /// </summary>
    public string StartStopKeybind { get; set; } = string.Empty;

    /// <summary>
    /// Target Key to be used in the autoinput.
    /// </summary>
    public string TargetInputKey { get; set; } = string.Empty;

    /// <summary>
    /// Flag to indicate the start run time is set.
    /// </summary>
    public bool ScheduleStartEnabled { get; set; }

    /// <summary>
    /// Time of schedule start.
    /// </summary>
    public DateTime ScheduleStartTime { get; set; }

    /// <summary>
    /// Flag to indicate the stop run time is set.
    /// </summary>
    public bool ScheduleStopEnabled { get; set; }

    /// <summary>
    /// Time of scheduled end.
    /// </summary>
    public DateTime ScheduleStopTime { get; set; }

    /// <summary>
    /// Currently seleted sequence from the list.
    /// Defaults to zero as a fallback.
    /// </summary>
    public int SelectedSequenceIndex { get; set; }

    /// <summary>
    /// Indicates wether the app is running the sequence or
    /// the single target key.
    /// </summary>
    public bool SequenceModeActive { get; set; }
}