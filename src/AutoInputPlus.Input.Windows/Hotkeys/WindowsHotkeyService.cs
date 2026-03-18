using System.ComponentModel;
using System.Runtime.InteropServices;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;
using AutoInputPlus.Input.Windows.Mapping;

namespace AutoInputPlus.Input.Windows.Hotkeys;

/// <summary>
/// Windows implementation of <see cref="IGlobalHotkey"/>.
/// This service manages a single active global hotkey for the application.
/// It must be initialized with a native window handle before registration.
/// </summary>
public sealed class WindowsHotkeyService : IGlobalHotkey
{
    private const int HotkeyId = 1;

    private nint _windowHandle;
    private bool _disposed;

    /// <inheritdoc/>
    public event EventHandler? HotkeyPressed;

    /// <inheritdoc/>
    public bool IsRegistered { get; private set; }

    /// <inheritdoc/>
    public Hotkey? CurrentHotkey { get; private set; }

    /// <inheritdoc/>
    public void Initialize(nint windowHandle)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (windowHandle == nint.Zero)
        {
            throw new ArgumentException("Window handle cannot be zero.", nameof(windowHandle));
        }

        if (_windowHandle != nint.Zero && _windowHandle != windowHandle)
        {
            throw new InvalidOperationException(
                "The global hotkey service has already been initialized with a different window handle.");
        }

        _windowHandle = windowHandle;
    }

    /// <inheritdoc/>
    public bool RegisterHotKey(Hotkey hotkey)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(hotkey);

        EnsureInitialized();

        if (!KeyCodeMapper.TryMapToVirtualKey(hotkey.Key, out ushort virtualKey))
        {
            throw new ArgumentException(
                $"Unsupported hotkey key token '{hotkey.Key}'.",
                nameof(hotkey));
        }

        uint nativeModifiers = HotkeyModifierMapper.ToNativeModifiers(hotkey.Modifiers);

        if (IsRegistered)
        {
            UnregisterHotKey();
        }

        bool registered = HotkeyNativeMethods.RegisterHotKey(
            hWnd: _windowHandle,
            id: HotkeyId,
            fsModifiers: nativeModifiers,
            vk: virtualKey);

        if (!registered)
        {
            int errorCode = Marshal.GetLastWin32Error();
            throw new Win32Exception(
                errorCode,
                $"Failed to register global hotkey '{hotkey}'.");
        }

        CurrentHotkey = hotkey;
        IsRegistered = true;

        return true;
    }

    /// <inheritdoc/>
    public bool UnregisterHotKey()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_windowHandle == nint.Zero || !IsRegistered)
        {
            CurrentHotkey = null;
            IsRegistered = false;
            return false;
        }

        bool unregistered = HotkeyNativeMethods.UnregisterHotKey(_windowHandle, HotkeyId);

        if (!unregistered)
        {
            int errorCode = Marshal.GetLastWin32Error();
            throw new Win32Exception(errorCode, "Failed to unregister global hotkey.");
        }

        CurrentHotkey = null;
        IsRegistered = false;

        return true;
    }

    /// <inheritdoc/>
    public bool HandleWindowMessage(uint message, nint wParam, nint lParam)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _ = lParam;

        if (!IsRegistered)
        {
            return false;
        }

        if (message != HotkeyNativeMethods.WmHotkey)
        {
            return false;
        }

        if (wParam != HotkeyId)
        {
            return false;
        }

        HotkeyPressed?.Invoke(this, EventArgs.Empty);
        return true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            if (_windowHandle != nint.Zero && IsRegistered)
            {
                HotkeyNativeMethods.UnregisterHotKey(_windowHandle, HotkeyId);
            }
        }
        finally
        {
            CurrentHotkey = null;
            IsRegistered = false;
            _windowHandle = nint.Zero;
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    private void EnsureInitialized()
    {
        if (_windowHandle == nint.Zero)
        {
            throw new InvalidOperationException(
                "The global hotkey service must be initialized with a window handle before registering a hotkey.");
        }
    }
}