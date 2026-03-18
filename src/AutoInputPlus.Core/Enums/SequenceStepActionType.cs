namespace AutoInputPlus.Core.Enums;

/// <summary>
/// Represents the type of action performed by a sequence step.
/// </summary>
public enum SequenceStepActionType
{
    /// <summary>
    /// Sends a full keyboard key press (down + up).
    /// </summary>
    KeyPress,

    /// <summary>
    /// Sends a keyboard key down event.
    /// </summary>
    KeyDown,

    /// <summary>
    /// Sends a keyboard key up event.
    /// </summary>
    KeyUp,

    /// <summary>
    /// Sends a full mouse click (down + up).
    /// </summary>
    MouseClick,

    /// <summary>
    /// Sends a mouse button down event.
    /// </summary>
    MouseDown,

    /// <summary>
    /// Sends a mouse button up event.
    /// </summary>
    MouseUp,

    /// <summary>
    /// Sends a mouse wheel event.
    /// Positive values scroll up and negative values scroll down.
    /// </summary>
    MouseWheel
}