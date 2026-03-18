using System.Runtime.InteropServices;

namespace AutoInputPlus.Input.Windows.Hotkeys;

/// <summary>
/// Native Win32 interop definitions for global hotkey registration.
/// </summary>
internal static class HotkeyNativeMethods
{
    /// <summary>
    /// Window message sent when a registered hotkey is pressed.
    /// </summary>
    internal const uint WmHotkey = 0x0312;

    /// <summary>
    /// Registers a system-wide hotkey.
    /// </summary>
    /// <param name="hWnd">The window handle that receives the hotkey message.</param>
    /// <param name="id">The hotkey identifier.</param>
    /// <param name="fsModifiers">Modifier flags.</param>
    /// <param name="vk">Virtual-key code.</param>
    /// <returns><see langword="true"/> if registration succeeded; otherwise, <see langword="false"/>.</returns>
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

    /// <summary>
    /// Unregisters a system-wide hotkey.
    /// </summary>
    /// <param name="hWnd">The window handle used during registration.</param>
    /// <param name="id">The hotkey identifier.</param>
    /// <returns><see langword="true"/> if unregistration succeeded; otherwise, <see langword="false"/>.</returns>
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnregisterHotKey(nint hWnd, int id);
}