namespace AutoInputPlus.Core.Enums;

/// <summary>
/// Represents the type of action performed by a sequence step.
/// </summary>
public enum SequenceStepActionType
{
    /// <summary>
    /// Sends a keyboard key press.
    /// </summary>
    KeyPress,

    /// <summary>
    /// Presses and holds a keyboard key.
    /// </summary>
    KeyDown,

    /// <summary>
    /// Releases a keyboard key that is currently held down.
    /// </summary>
    KeyUp,

    /// <summary>
    /// Sends a mouse button click.
    /// </summary>
    MouseClick,

    /// <summary>
    /// Presses and holds a mouse button.
    /// </summary>
    MouseDown,

    /// <summary>
    /// Releases a mouse button that is currently held down.
    /// </summary>
    MouseUp,

    /// <summary>
    /// Waits for the specified delay without sending input.
    /// </summary>
    Delay
}