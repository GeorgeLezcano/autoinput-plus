using AutoInputPlus.Core.Enums;

namespace AutoInputPlus.Core.Interfaces;

/// <summary>
/// Defines operations for registering and unregistering a global hotkey.
/// </summary>
public interface IGlobalHotkey
{
    /// <summary>
    /// Occurs when the registered global hotkey is triggered.
    /// </summary>
    event EventHandler? HotkeyPressed;

    /// <summary>
    /// Registers a global hotkey.
    /// </summary>
    /// <param name="key">
    /// The application key token to register, such as <c>F8</c> or <c>F10</c>.
    /// </param>
    /// <param name="modifiers">
    /// Optional modifier keys that must be held with the key.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if registration succeeded; otherwise, <see langword="false"/>.
    /// </returns>
    bool RegisterHotKey(string key, HotkeyModifiers modifiers = HotkeyModifiers.None);

    /// <summary>
    /// Unregisters the currently registered global hotkey.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if unregistration succeeded; otherwise, <see langword="false"/>.
    /// </returns>
    bool UnregisterHotKey();
}