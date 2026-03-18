
namespace AutoInputPlus.Input.Windows.Mapping;

/// <summary>
/// Maps application key tokens to Windows virtual-key codes.
/// </summary>
/// <remarks>
/// This type is static because it provides deterministic lookup behavior and does
/// not currently require configuration or external dependencies.
/// </remarks>
public static class KeyCodeMapper
{
    private static readonly Dictionary<string, ushort> KeyMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Common control keys
            ["Backspace"] = 0x08,
            ["Tab"] = 0x09,
            ["Enter"] = 0x0D,
            ["Return"] = 0x0D,
            ["Shift"] = 0x10,
            ["Ctrl"] = 0x11,
            ["Control"] = 0x11,
            ["Alt"] = 0x12,
            ["Pause"] = 0x13,
            ["CapsLock"] = 0x14,
            ["Escape"] = 0x1B,
            ["Esc"] = 0x1B,
            ["Space"] = 0x20,
            ["PageUp"] = 0x21,
            ["PageDown"] = 0x22,
            ["End"] = 0x23,
            ["Home"] = 0x24,
            ["Left"] = 0x25,
            ["Up"] = 0x26,
            ["Right"] = 0x27,
            ["Down"] = 0x28,
            ["PrintScreen"] = 0x2C,
            ["PrtSc"] = 0x2C,
            ["Insert"] = 0x2D,
            ["Delete"] = 0x2E,
            ["Del"] = 0x2E,

            // Windows keys
            ["LWin"] = 0x5B,
            ["LeftWin"] = 0x5B,
            ["RWin"] = 0x5C,
            ["RightWin"] = 0x5C,
            ["Win"] = 0x5B,
            ["Windows"] = 0x5B,

            // Numpad
            ["NumPad0"] = 0x60,
            ["NumPad1"] = 0x61,
            ["NumPad2"] = 0x62,
            ["NumPad3"] = 0x63,
            ["NumPad4"] = 0x64,
            ["NumPad5"] = 0x65,
            ["NumPad6"] = 0x66,
            ["NumPad7"] = 0x67,
            ["NumPad8"] = 0x68,
            ["NumPad9"] = 0x69,
            ["Multiply"] = 0x6A,
            ["Add"] = 0x6B,
            ["Separator"] = 0x6C,
            ["Subtract"] = 0x6D,
            ["Decimal"] = 0x6E,
            ["Divide"] = 0x6F,

            // Function keys
            ["F1"] = 0x70,
            ["F2"] = 0x71,
            ["F3"] = 0x72,
            ["F4"] = 0x73,
            ["F5"] = 0x74,
            ["F6"] = 0x75,
            ["F7"] = 0x76,
            ["F8"] = 0x77,
            ["F9"] = 0x78,
            ["F10"] = 0x79,
            ["F11"] = 0x7A,
            ["F12"] = 0x7B,

            // Explicit left/right modifiers
            ["LeftShift"] = 0xA0,
            ["RightShift"] = 0xA1,
            ["LeftCtrl"] = 0xA2,
            ["LeftControl"] = 0xA2,
            ["RightCtrl"] = 0xA3,
            ["RightControl"] = 0xA3,
            ["LeftAlt"] = 0xA4,
            ["RightAlt"] = 0xA5
        };

    /// <summary>
    /// Attempts to map an application key token to a Windows virtual-key code.
    /// </summary>
    /// <param name="key">
    /// The application key token, such as <c>A</c>, <c>Enter</c>, or <c>F8</c>.
    /// </param>
    /// <param name="virtualKeyCode">
    /// When this method returns, contains the resolved Windows virtual-key code
    /// if the mapping succeeded; otherwise, <c>0</c>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the key token is supported; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryMapToVirtualKey(string key, out ushort virtualKeyCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        string normalizedKey = NormalizeKey(key);

        if (TryMapSingleLetter(normalizedKey, out virtualKeyCode))
        {
            return true;
        }

        if (TryMapSingleDigit(normalizedKey, out virtualKeyCode))
        {
            return true;
        }

        return KeyMap.TryGetValue(normalizedKey, out virtualKeyCode);
    }

    private static string NormalizeKey(string key) => key.Trim();

    private static bool TryMapSingleLetter(string key, out ushort virtualKeyCode)
    {
        virtualKeyCode = 0;

        if (key.Length != 1)
        {
            return false;
        }

        char c = char.ToUpperInvariant(key[0]);
        if (c is >= 'A' and <= 'Z')
        {
            virtualKeyCode = c;
            return true;
        }

        return false;
    }

    private static bool TryMapSingleDigit(string key, out ushort virtualKeyCode)
    {
        virtualKeyCode = 0;

        if (key.Length != 1)
        {
            return false;
        }

        char c = key[0];
        if (c is >= '0' and <= '9')
        {
            virtualKeyCode = c;
            return true;
        }

        return false;
    }
}