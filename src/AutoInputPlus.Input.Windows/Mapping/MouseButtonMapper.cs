using AutoInputPlus.Core.Enums;
using AutoInputPlus.Input.Windows.Input;

namespace AutoInputPlus.Input.Windows.Mapping;

/// <summary>
/// Maps application mouse buttons to native Win32 mouse event flags.
/// </summary>
internal static class MouseButtonMapper
{
    /// <summary>
    /// Gets the native mouse-down flag for the specified button.
    /// </summary>
    /// <param name="button">The application mouse button.</param>
    /// <returns>The Win32 mouse-down flag.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the mouse button is not supported.
    /// </exception>
    public static uint GetMouseDownFlag(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => NativeMethods.MouseEventFLeftDown,
            MouseButton.Right => NativeMethods.MouseEventFRightDown,
            MouseButton.Middle => NativeMethods.MouseEventFMiddleDown,
            _ => throw new ArgumentOutOfRangeException(nameof(button), button, "Unsupported mouse button.")
        };
    }

    /// <summary>
    /// Gets the native mouse-up flag for the specified button.
    /// </summary>
    /// <param name="button">The application mouse button.</param>
    /// <returns>The Win32 mouse-up flag.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the mouse button is not supported.
    /// </exception>
    public static uint GetMouseUpFlag(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => NativeMethods.MouseEventFLeftUp,
            MouseButton.Right => NativeMethods.MouseEventFRightUp,
            MouseButton.Middle => NativeMethods.MouseEventFMiddleUp,
            _ => throw new ArgumentOutOfRangeException(nameof(button), button, "Unsupported mouse button.")
        };
    }
}