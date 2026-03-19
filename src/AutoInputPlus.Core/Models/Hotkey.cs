using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Extensions;

namespace AutoInputPlus.Core.Models;

/// <summary>
/// Represents a single keyboard global hotkey definition for the application.
/// This model is intentionally keyboard-only because the current Windows implementation
/// is based on the Win32 RegisterHotKey API, which does not support mouse buttons.
/// </summary>
/// <param name="Key">
/// The keyboard key used by the hotkey.
/// </param>
/// <param name="Modifiers">
/// The modifier keys required for the hotkey.
/// </param>
public sealed record Hotkey(InputKey Key, HotkeyModifiers Modifiers = HotkeyModifiers.None)
{
    /// <summary>
    /// Gets the keyboard key used by the hotkey.
    /// </summary>
    public InputKey Key { get; init; } = Key;

    /// <summary>
    /// Gets the required modifier keys.
    /// </summary>
    public HotkeyModifiers Modifiers { get; init; } = Modifiers;

    /// <summary>
    /// Returns a display string for the hotkey.
    /// </summary>
    /// <returns>A display string such as <c>Control+F8</c> or <c>F8</c>.</returns>
    public override string ToString()
    {
        string keyDisplay = Key.ToDisplayName();

        return Modifiers == HotkeyModifiers.None
            ? keyDisplay
            : $"{Modifiers}+{keyDisplay}";
    }
}