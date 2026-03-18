using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Core.Interfaces;

/// <summary>
/// Defines operations for managing a single global hotkey registration
/// for the application.
/// This contract is designed for a single active hotkey at a time.
/// Registering a new hotkey replaces any previously registered hotkey.
/// </summary>
public interface IGlobalHotkey : IDisposable
{
    /// <summary>
    /// Occurs when the currently registered global hotkey is triggered.
    /// </summary>
    event EventHandler? HotkeyPressed;

    /// <summary>
    /// Gets a value indicating whether a hotkey is currently registered.
    /// </summary>
    bool IsRegistered { get; }

    /// <summary>
    /// Gets the currently registered hotkey definition, if any.
    /// </summary>
    Hotkey? CurrentHotkey { get; }

    /// <summary>
    /// Initializes the service with the native window handle that will receive
    /// the hotkey message.
    /// </summary>
    /// <param name="windowHandle">The native window handle.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="windowHandle"/> is zero.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the service is initialized more than once with a different handle.
    /// </exception>
    void Initialize(nint windowHandle);

    /// <summary>
    /// Registers the specified global hotkey.
    /// </summary>
    /// <param name="hotkey">The hotkey definition to register.</param>
    /// <returns>
    /// <see langword="true"/> when the hotkey is successfully registered.
    /// </returns>
    /// <remarks>
    /// If another hotkey is already registered, it is unregistered first and replaced.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="hotkey"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the hotkey key token is invalid or unsupported.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the service has not been initialized with a window handle.
    /// </exception>
    bool RegisterHotKey(Hotkey hotkey);

    /// <summary>
    /// Unregisters the currently registered global hotkey.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if a hotkey was actively registered and was removed;
    /// otherwise, <see langword="false"/> when there was nothing to unregister.
    /// </returns>
    bool UnregisterHotKey();

    /// <summary>
    /// Handles a native window message and raises <see cref="HotkeyPressed"/>
    /// when the registered hotkey is triggered.
    /// </summary>
    /// <param name="message">The native window message identifier.</param>
    /// <param name="wParam">The native message identifier value.</param>
    /// <param name="lParam">
    /// The native hotkey message payload. This value is currently not interpreted
    /// by the service and is accepted for message-forwarding completeness.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the message represented the registered hotkey
    /// and was handled by this service; otherwise, <see langword="false"/>.
    /// </returns>
    bool HandleWindowMessage(uint message, nint wParam, nint lParam);
}