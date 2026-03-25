using System.Diagnostics.CodeAnalysis;
using AutoInputPlus.Core.Enums;

namespace AutoInputPlus.Core.Models;

/// <summary>
/// Represents a single executable step within a sequence.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class SequenceStep
{
    /// <summary>
    /// Gets or sets the display name of the step.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the selected target input type for the step.
    /// </summary>
    public SequenceStepTargetType TargetType { get; set; } = SequenceStepTargetType.Keyboard;

    /// <summary>
    /// Gets or sets the keyboard key used when <see cref="TargetType"/> is <see cref="SequenceStepTargetType.Keyboard"/>.
    /// </summary>
    public InputKey? Key { get; set; }

    /// <summary>
    /// Gets or sets the mouse button used when <see cref="TargetType"/> is <see cref="SequenceStepTargetType.MouseButton"/>.
    /// </summary>
    public MouseButton? MouseButton { get; set; }

    /// <summary>
    /// Gets or sets the mouse wheel delta used when <see cref="TargetType"/> is <see cref="SequenceStepTargetType.MouseWheel"/>.
    /// Positive values scroll up and negative values scroll down.
    /// </summary>
    public int MouseWheelDelta { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the target input should be held
    /// for the configured duration instead of performing a single press or click.
    /// </summary>
    public bool IsHold { get; set; }

    /// <summary>
    /// Gets or sets the duration in milliseconds for hold actions.
    /// This value is ignored when <see cref="IsHold"/> is false.
    /// </summary>
    public int DurationMilliseconds { get; set; }

    /// <summary>
    /// Gets or sets the delay in milliseconds after executing the step.
    /// </summary>
    public int DelayAfterMilliseconds { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the step is enabled.
    /// Disabled steps are skipped during execution.
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}