using AutoInputPlus.Core.Enums;

namespace AutoInputPlus.Core.Models;

/// <summary>
/// Represents a single global hotkey definition for the application.
/// </summary>
/// <param name="Key">
/// The application key token, such as <c>F8</c>, <c>F10</c>, or <c>A</c>.
/// </param>
/// <param name="Modifiers">
/// The modifier keys required for the hotkey.
/// </param>
public sealed record Hotkey(string Key, HotkeyModifiers Modifiers = HotkeyModifiers.None)
{
    /// <summary>
    /// Gets the normalized key token.
    /// </summary>
    public string Key { get; init; } = NormalizeKey(Key);

    /// <summary>
    /// Gets the required modifier keys.
    /// </summary>
    public HotkeyModifiers Modifiers { get; init; } = Modifiers;

    private static string NormalizeKey(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return key.Trim();
    }

    /// <summary>
    /// Returns a display string for the hotkey.
    /// </summary>
    /// <returns>A display string such as <c>Control+F8</c> or <c>F8</c>.</returns>
    public override string ToString()
    {
        return Modifiers == HotkeyModifiers.None
            ? Key
            : $"{Modifiers}+{Key}";
    }
}