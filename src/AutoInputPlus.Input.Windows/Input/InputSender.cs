using System.ComponentModel;
using System.Runtime.InteropServices;
using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Input.Windows.Mapping;

namespace AutoInputPlus.Input.Windows.Input;

/// <summary>
/// Windows implementation of <see cref="IInputSender"/>.
/// This class translates application-level keyboard and mouse requests into
/// Win32 <c>SendInput</c> calls.
/// </summary>
public sealed class InputSender : IInputSender
{
    /// <inheritdoc/>
    public void KeyPress(InputKey key)
    {
        KeyDown(key);
        KeyUp(key);
    }

    /// <inheritdoc/>
    public void KeyDown(InputKey key)
    {
        ushort virtualKey = ResolveVirtualKey(key);

        NativeMethods.INPUT input = CreateKeyboardInput(virtualKey, keyUp: false);
        SendSingleInput(input, $"Failed to send key down for '{key}'.");
    }

    /// <inheritdoc/>
    public void KeyUp(InputKey key)
    {
        ushort virtualKey = ResolveVirtualKey(key);

        NativeMethods.INPUT input = CreateKeyboardInput(virtualKey, keyUp: true);
        SendSingleInput(input, $"Failed to send key up for '{key}'.");
    }

    /// <inheritdoc/>
    public void MouseClick(MouseButton button)
    {
        MouseDown(button);
        MouseUp(button);
    }

    /// <inheritdoc/>
    public void MouseDown(MouseButton button)
    {
        uint flag = MouseButtonMapper.GetMouseDownFlag(button);

        NativeMethods.INPUT input = CreateMouseButtonInput(flag);
        SendSingleInput(input, $"Failed to send mouse down for '{button}'.");
    }

    /// <inheritdoc/>
    public void MouseUp(MouseButton button)
    {
        uint flag = MouseButtonMapper.GetMouseUpFlag(button);

        NativeMethods.INPUT input = CreateMouseButtonInput(flag);
        SendSingleInput(input, $"Failed to send mouse up for '{button}'.");
    }

    /// <inheritdoc/>
    public void MouseWheel(int delta)
    {
        if (delta == 0) return;

        NativeMethods.INPUT input = CreateMouseWheelInput(delta);
        SendSingleInput(input, $"Failed to send mouse wheel input for delta '{delta}'.");
    }

    private static ushort ResolveVirtualKey(InputKey key)
    {
        if (!KeyCodeMapper.TryMapToVirtualKey(key, out ushort virtualKey))
        {
            throw new ArgumentException($"Unsupported input key '{key}'.", nameof(key));
        }

        return virtualKey;
    }

    private static NativeMethods.INPUT CreateKeyboardInput(ushort virtualKey, bool keyUp)
    {
        return new NativeMethods.INPUT
        {
            type = NativeMethods.InputKeyboard,
            U = new NativeMethods.InputUnion
            {
                ki = new NativeMethods.KEYBDINPUT
                {
                    wVk = virtualKey,
                    wScan = 0,
                    dwFlags = keyUp ? NativeMethods.KeyEventFKeyUp : 0,
                    time = 0,
                    dwExtraInfo = nint.Zero
                }
            }
        };
    }

    private static NativeMethods.INPUT CreateMouseButtonInput(uint flag)
    {
        return new NativeMethods.INPUT
        {
            type = NativeMethods.InputMouse,
            U = new NativeMethods.InputUnion
            {
                mi = new NativeMethods.MOUSEINPUT
                {
                    dx = 0,
                    dy = 0,
                    mouseData = 0,
                    dwFlags = flag,
                    time = 0,
                    dwExtraInfo = nint.Zero
                }
            }
        };
    }

    private static NativeMethods.INPUT CreateMouseWheelInput(int delta)
    {
        return new NativeMethods.INPUT
        {
            type = NativeMethods.InputMouse,
            U = new NativeMethods.InputUnion
            {
                mi = new NativeMethods.MOUSEINPUT
                {
                    dx = 0,
                    dy = 0,
                    mouseData = unchecked((uint)delta),
                    dwFlags = NativeMethods.MouseEventFWheel,
                    time = 0,
                    dwExtraInfo = nint.Zero
                }
            }
        };
    }

    private static void SendSingleInput(NativeMethods.INPUT input, string failureMessage)
    {
        NativeMethods.INPUT[] inputs = [input];

        uint sent = NativeMethods.SendInput(
            nInputs: 1,
            pInputs: inputs,
            cbSize: Marshal.SizeOf<NativeMethods.INPUT>());

        if (sent == 0)
        {
            int errorCode = Marshal.GetLastWin32Error();
            throw new Win32Exception(errorCode, failureMessage);
        }
    }
}