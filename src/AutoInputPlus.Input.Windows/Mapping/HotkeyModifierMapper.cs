using AutoInputPlus.Core.Enums;

namespace AutoInputPlus.Input.Windows.Mapping;

/// <summary>
/// Maps application hotkey modifiers to Win32 modifier flags.
/// </summary>
internal static class HotkeyModifierMapper
{
    private const uint ModAlt = 0x0001;
    private const uint ModControl = 0x0002;
    private const uint ModShift = 0x0004;
    private const uint ModWin = 0x0008;

    /// <summary>
    /// Converts the application modifier flags to native Win32 modifier flags.
    /// </summary>
    /// <param name="modifiers">The application modifier flags.</param>
    /// <returns>The native Win32 modifier flags.</returns>
    public static uint ToNativeModifiers(HotkeyModifiers modifiers)
    {
        uint result = 0;

        if (modifiers.HasFlag(HotkeyModifiers.Alt))
        {
            result |= ModAlt;
        }

        if (modifiers.HasFlag(HotkeyModifiers.Control))
        {
            result |= ModControl;
        }

        if (modifiers.HasFlag(HotkeyModifiers.Shift))
        {
            result |= ModShift;
        }

        if (modifiers.HasFlag(HotkeyModifiers.Windows))
        {
            result |= ModWin;
        }

        return result;
    }
}