using AutoInputPlus.Core.Enums;

namespace AutoInputPlus.Core.Models;

/// <summary>
/// Represents a single executable step within a sequence.
/// </summary>
public sealed class SequenceStep
{
    /// <summary>
    /// Gets or sets the display name of the step.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of action performed by the step.
    /// </summary>
    public SequenceStepActionType ActionType { get; set; } = SequenceStepActionType.KeyPress;

    /// <summary>
    /// Gets or sets the key token used by keyboard actions.
    /// </summary>
    /// <remarks>
    /// Used for <see cref="SequenceStepActionType.KeyPress"/>,
    /// <see cref="SequenceStepActionType.KeyDown"/>, and
    /// <see cref="SequenceStepActionType.KeyUp"/>.
    /// </remarks>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the mouse button used by mouse button actions.
    /// </summary>
    /// <remarks>
    /// Used for <see cref="SequenceStepActionType.MouseClick"/>,
    /// <see cref="SequenceStepActionType.MouseDown"/>, and
    /// <see cref="SequenceStepActionType.MouseUp"/>.
    /// </remarks>
    public MouseButton? MouseButton { get; set; }

    /// <summary>
    /// Gets or sets the mouse wheel delta used by wheel actions.
    /// </summary>
    /// <remarks>
    /// Positive values scroll up and negative values scroll down.
    /// Used for <see cref="SequenceStepActionType.MouseWheel"/>.
    /// </remarks>
    public int MouseWheelDelta { get; set; }

    /// <summary>
    /// Gets or sets the delay in milliseconds before executing the step.
    /// </summary>
    public int DelayBeforeMilliseconds { get; set; }

    /// <summary>
    /// Gets or sets the delay in milliseconds after executing the step.
    /// </summary>
    public int DelayAfterMilliseconds { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the step remains held
    /// during the post-action delay when that behavior is applicable.
    /// </summary>
    /// <remarks>
    /// This value is mainly meaningful for press-and-hold style actions such as
    /// <see cref="SequenceStepActionType.KeyDown"/> and
    /// <see cref="SequenceStepActionType.MouseDown"/>.
    /// </remarks>
    public bool HoldDuringDelay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the step is enabled.
    /// Disabled steps are skipped during execution.
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}