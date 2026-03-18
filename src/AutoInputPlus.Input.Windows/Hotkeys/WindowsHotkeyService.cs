using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Interfaces;

namespace AutoInputPlus.Input.Windows.Hotkeys;

/// <summary>
/// Windows implementation of <see cref="IGlobalHotkey"/>.
/// </summary>
/// <remarks>
/// This class will later wrap Win32 hotkey registration and message handling.
/// </remarks>
public sealed class WindowsHotkeyService : IGlobalHotkey
{
    /// <inheritdoc/>
    public event EventHandler? HotkeyPressed;

    /// <inheritdoc/>
    public bool RegisterHotKey(string key, HotkeyModifiers modifiers = HotkeyModifiers.None)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool UnregisterHotKey()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Raises the <see cref="HotkeyPressed"/> event.
    /// </summary>
    internal void OnHotkeyPressed()
    {
        HotkeyPressed?.Invoke(this, EventArgs.Empty);
    }
}