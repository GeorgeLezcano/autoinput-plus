namespace AutoInputPlus.Core.Enums;

/// <summary>
/// Represents modifier keys used when registering a global hotkey.
/// </summary>
[Flags]
public enum HotkeyModifiers
{
    /// <summary>
    /// No modifier keys.
    /// </summary>
    None = 0,

    /// <summary>
    /// The Alt key.
    /// </summary>
    Alt = 1,

    /// <summary>
    /// The Control key.
    /// </summary>
    Control = 2,

    /// <summary>
    /// The Shift key.
    /// </summary>
    Shift = 4,

    /// <summary>
    /// The Windows key.
    /// </summary>
    Windows = 8
}