using AutoInputPlus.Core.Enums;

namespace AutoInputPlus.Core.Models;

/// <summary>
/// Represents a single executable step within a sequence,
/// such as a keyboard or mouse input action.
/// </summary>
public sealed class SequenceStep
{
    /// <summary>
    /// Gets or sets the display name of the step.
    /// </summary>
    public string Name { get; set; } = string.Empty; //TODO Default name plan is for it to have a format of "Step {0}" where {0} represents List<SequenceStep> index + 1

    /// <summary>
    /// Gets or sets the type of action performed by the step.
    /// </summary>
    public SequenceStepActionType ActionType { get; set; } = SequenceStepActionType.KeyPress;

    /// <summary>
    /// Gets or sets the target value used by the step,
    /// such as a keyboard key or mouse button name.
    /// </summary>
    public string Target { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the delay in milliseconds before executing the step.
    /// </summary>
    public int DelayBeforeMilliseconds { get; set; }

    /// <summary>
    /// Gets or sets the delay in milliseconds after executing the step.
    /// <see cref="HoldDuringDelay"/> influences this delay.
    /// </summary>
    public int DelayAfterMilliseconds { get; set; }

    /// <summary>
    /// Flag that indicates if the key or mouse input
    /// should be held during the delay after the 
    /// action is performed.
    /// </summary>
    public bool HoldDuringDelay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the step is enabled.
    /// Disabled steps are skipped during execution.
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}