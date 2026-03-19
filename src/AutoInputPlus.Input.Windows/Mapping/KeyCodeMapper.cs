using AutoInputPlus.Core.Enums;

namespace AutoInputPlus.Input.Windows.Mapping;

/// <summary>
/// Maps supported application input keys to Windows virtual-key codes.
/// </summary>
/// <remarks>
/// This type is static because it provides deterministic lookup behavior and does
/// not currently require configuration or external dependencies.
/// </remarks>
public static class KeyCodeMapper
{
    /// <summary>
    /// Attempts to map a supported application input key to a Windows virtual-key code.
    /// </summary>
    /// <param name="key">The supported application input key.</param>
    /// <param name="virtualKeyCode">
    /// When this method returns, contains the resolved Windows virtual-key code
    /// if the mapping succeeded; otherwise, <c>0</c>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the key is supported; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryMapToVirtualKey(InputKey key, out ushort virtualKeyCode)
        => KeyMap.TryGetValue(key, out virtualKeyCode);

    private static readonly Dictionary<InputKey, ushort> KeyMap = new()
    {
        // Letters
        [InputKey.A] = 0x41,
        [InputKey.B] = 0x42,
        [InputKey.C] = 0x43,
        [InputKey.D] = 0x44,
        [InputKey.E] = 0x45,
        [InputKey.F] = 0x46,
        [InputKey.G] = 0x47,
        [InputKey.H] = 0x48,
        [InputKey.I] = 0x49,
        [InputKey.J] = 0x4A,
        [InputKey.K] = 0x4B,
        [InputKey.L] = 0x4C,
        [InputKey.M] = 0x4D,
        [InputKey.N] = 0x4E,
        [InputKey.O] = 0x4F,
        [InputKey.P] = 0x50,
        [InputKey.Q] = 0x51,
        [InputKey.R] = 0x52,
        [InputKey.S] = 0x53,
        [InputKey.T] = 0x54,
        [InputKey.U] = 0x55,
        [InputKey.V] = 0x56,
        [InputKey.W] = 0x57,
        [InputKey.X] = 0x58,
        [InputKey.Y] = 0x59,
        [InputKey.Z] = 0x5A,

        // Top-row digits
        [InputKey.D0] = 0x30,
        [InputKey.D1] = 0x31,
        [InputKey.D2] = 0x32,
        [InputKey.D3] = 0x33,
        [InputKey.D4] = 0x34,
        [InputKey.D5] = 0x35,
        [InputKey.D6] = 0x36,
        [InputKey.D7] = 0x37,
        [InputKey.D8] = 0x38,
        [InputKey.D9] = 0x39,

        // Common control keys
        [InputKey.Backspace] = 0x08,
        [InputKey.Tab] = 0x09,
        [InputKey.Enter] = 0x0D,
        [InputKey.Pause] = 0x13,
        [InputKey.CapsLock] = 0x14,
        [InputKey.Escape] = 0x1B,
        [InputKey.Space] = 0x20,

        // Navigation
        [InputKey.PageUp] = 0x21,
        [InputKey.PageDown] = 0x22,
        [InputKey.End] = 0x23,
        [InputKey.Home] = 0x24,
        [InputKey.Left] = 0x25,
        [InputKey.Up] = 0x26,
        [InputKey.Right] = 0x27,
        [InputKey.Down] = 0x28,
        [InputKey.PrintScreen] = 0x2C,
        [InputKey.Insert] = 0x2D,
        [InputKey.Delete] = 0x2E,

        // Windows keys
        [InputKey.LeftWin] = 0x5B,
        [InputKey.RightWin] = 0x5C,

        // Numpad
        [InputKey.NumPad0] = 0x60,
        [InputKey.NumPad1] = 0x61,
        [InputKey.NumPad2] = 0x62,
        [InputKey.NumPad3] = 0x63,
        [InputKey.NumPad4] = 0x64,
        [InputKey.NumPad5] = 0x65,
        [InputKey.NumPad6] = 0x66,
        [InputKey.NumPad7] = 0x67,
        [InputKey.NumPad8] = 0x68,
        [InputKey.NumPad9] = 0x69,
        [InputKey.Multiply] = 0x6A,
        [InputKey.Add] = 0x6B,
        [InputKey.Separator] = 0x6C,
        [InputKey.Subtract] = 0x6D,
        [InputKey.Decimal] = 0x6E,
        [InputKey.Divide] = 0x6F,

        // Function keys
        [InputKey.F1] = 0x70,
        [InputKey.F2] = 0x71,
        [InputKey.F3] = 0x72,
        [InputKey.F4] = 0x73,
        [InputKey.F5] = 0x74,
        [InputKey.F6] = 0x75,
        [InputKey.F7] = 0x76,
        [InputKey.F8] = 0x77,
        [InputKey.F9] = 0x78,
        [InputKey.F10] = 0x79,
        [InputKey.F11] = 0x7A,
        [InputKey.F12] = 0x7B,

        // Explicit modifiers as keys
        [InputKey.LeftShift] = 0xA0,
        [InputKey.RightShift] = 0xA1,
        [InputKey.LeftCtrl] = 0xA2,
        [InputKey.RightCtrl] = 0xA3,
        [InputKey.LeftAlt] = 0xA4,
        [InputKey.RightAlt] = 0xA5
    };

}