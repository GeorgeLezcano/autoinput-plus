using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using AutoInputPlus.Core.Constants;
using AutoInputPlus.Core.Enums;

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
    /// Gets or sets the interval in milliseconds between repeated inputs.
    /// </summary>
    public int IntervalMilliseconds { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether execution should continue until manually stopped.
    /// </summary>
    public bool RunUntilStopActive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether execution should stop after a configured count.
    /// </summary>
    public bool RunUntilSetCountActive { get; set; }

    /// <summary>
    /// Gets or sets the total input count to execute before stopping when count-based execution is enabled.
    /// </summary>
    public int StopInputCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the target input should be held instead of pressed and released.
    /// </summary>
    public bool HoldTargetEnabled { get; set; }

    /// <summary>
    /// Gets or sets the user-configurable keyboard hotkey used to start or stop execution.
    /// </summary>
    public Hotkey StartStopHotkey { get; set; } = new(InputKey.F8);

    /// <summary>
    /// Gets or sets the target input binding used during single-input execution mode.
    /// This binding may target either a keyboard key or a mouse button.
    /// </summary>
    public InputBinding TargetInputBinding { get; set; } = InputBinding.FromMouseButton(MouseButton.Left);

    /// <summary>
    /// Gets or sets a value indicating whether the scheduled start time is enabled.
    /// </summary>
    public bool ScheduleStartEnabled { get; set; }

    /// <summary>
    /// Gets or sets the scheduled start time.
    /// </summary>
    public DateTime ScheduleStartTime { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the scheduled stop time is enabled.
    /// </summary>
    public bool ScheduleStopEnabled { get; set; }

    /// <summary>
    /// Gets or sets the scheduled stop time.
    /// </summary>
    public DateTime ScheduleStopTime { get; set; }

    /// <summary>
    /// Gets or sets the currently selected sequence index.
    /// Defaults to zero as a fallback.
    /// </summary>
    public int SelectedSequenceIndex { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether execution should use the selected sequence
    /// instead of the single target input binding.
    /// </summary>
    public bool SequenceModeActive { get; set; }
}