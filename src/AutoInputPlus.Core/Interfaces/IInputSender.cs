using AutoInputPlus.Core.Enums;

namespace AutoInputPlus.Core.Interfaces;

/// <summary>
/// Defines low-level input operations for sending keyboard and mouse input.
/// Platform-specific projects implement this contract.
/// </summary>
public interface IInputSender
{
    /// <summary>
    /// Sends a full key press for the specified key.
    /// </summary>
    /// <param name="key">
    /// The application key token to send, such as <c>A</c>, <c>Enter</c>, or <c>F5</c>.
    /// </param>
    void KeyPress(string key);

    /// <summary>
    /// Sends a key down event for the specified key.
    /// </summary>
    /// <param name="key">
    /// The application key token to send.
    /// </param>
    void KeyDown(string key);

    /// <summary>
    /// Sends a key up event for the specified key.
    /// </summary>
    /// <param name="key">
    /// The application key token to send.
    /// </param>
    void KeyUp(string key);

    /// <summary>
    /// Sends a full mouse click for the specified button.
    /// </summary>
    /// <param name="button">
    /// The mouse button to click.
    /// </param>
    void MouseClick(MouseButton button);

    /// <summary>
    /// Sends a mouse button down event for the specified button.
    /// </summary>
    /// <param name="button">
    /// The mouse button to press.
    /// </param>
    void MouseDown(MouseButton button);

    /// <summary>
    /// Sends a mouse button up event for the specified button.
    /// </summary>
    /// <param name="button">
    /// The mouse button to release.
    /// </param>
    void MouseUp(MouseButton button);

    /// <summary>
    /// Sends a mouse wheel event.
    /// </summary>
    /// <param name="delta">
    /// The wheel delta. Positive values scroll up and negative values scroll down.
    /// </param>
    void MouseWheel(int delta);
}