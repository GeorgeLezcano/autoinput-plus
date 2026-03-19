using AutoInputPlus.Core.Enums;

namespace AutoInputPlus.Core.Extensions;

/// <summary>
/// Provides display helpers for <see cref="InputKey"/> values.
/// </summary>
public static class InputKeyExtensions
{
    /// <summary>
    /// Converts an <see cref="InputKey"/> value to a user-friendly display name.
    /// </summary>
    /// <param name="key">The input key to format.</param>
    /// <returns>A user-friendly display name.</returns>
    public static string ToDisplayName(this InputKey key)
    {
        return key switch
        {
            InputKey.D0 => "0",
            InputKey.D1 => "1",
            InputKey.D2 => "2",
            InputKey.D3 => "3",
            InputKey.D4 => "4",
            InputKey.D5 => "5",
            InputKey.D6 => "6",
            InputKey.D7 => "7",
            InputKey.D8 => "8",
            InputKey.D9 => "9",

            InputKey.Backspace => "Backspace",
            InputKey.Tab => "Tab",
            InputKey.Enter => "Enter",
            InputKey.Pause => "Pause",
            InputKey.CapsLock => "Caps Lock",
            InputKey.Escape => "Escape",
            InputKey.Space => "Space",

            InputKey.PageUp => "Page Up",
            InputKey.PageDown => "Page Down",
            InputKey.End => "End",
            InputKey.Home => "Home",
            InputKey.Left => "Left Arrow",
            InputKey.Up => "Up Arrow",
            InputKey.Right => "Right Arrow",
            InputKey.Down => "Down Arrow",
            InputKey.Insert => "Insert",
            InputKey.Delete => "Delete",
            InputKey.PrintScreen => "Print Screen",

            InputKey.LeftWin => "Left Win",
            InputKey.RightWin => "Right Win",

            InputKey.NumPad0 => "NumPad 0",
            InputKey.NumPad1 => "NumPad 1",
            InputKey.NumPad2 => "NumPad 2",
            InputKey.NumPad3 => "NumPad 3",
            InputKey.NumPad4 => "NumPad 4",
            InputKey.NumPad5 => "NumPad 5",
            InputKey.NumPad6 => "NumPad 6",
            InputKey.NumPad7 => "NumPad 7",
            InputKey.NumPad8 => "NumPad 8",
            InputKey.NumPad9 => "NumPad 9",
            InputKey.Multiply => "NumPad *",
            InputKey.Add => "NumPad +",
            InputKey.Separator => "NumPad Separator",
            InputKey.Subtract => "NumPad -",
            InputKey.Decimal => "NumPad .",
            InputKey.Divide => "NumPad /",

            InputKey.LeftShift => "Left Shift",
            InputKey.RightShift => "Right Shift",
            InputKey.LeftCtrl => "Left Ctrl",
            InputKey.RightCtrl => "Right Ctrl",
            InputKey.LeftAlt => "Left Alt",
            InputKey.RightAlt => "Right Alt",

            _ => key.ToString()
        };
    }
}