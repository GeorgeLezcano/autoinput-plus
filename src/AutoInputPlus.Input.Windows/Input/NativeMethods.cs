using System.Runtime.InteropServices;

namespace AutoInputPlus.Input.Windows.Input;

/// <summary>
/// Native Win32 interop definitions for sending keyboard and mouse input.
/// </summary>
internal static class NativeMethods
{
    /// <summary>
    /// Keyboard input type.
    /// </summary>
    internal const uint InputKeyboard = 1;

    /// <summary>
    /// Mouse input type.
    /// </summary>
    internal const uint InputMouse = 0;

    /// <summary>
    /// Key is being released.
    /// </summary>
    internal const uint KeyEventFKeyUp = 0x0002;

    /// <summary>
    /// Left mouse button down.
    /// </summary>
    internal const uint MouseEventFLeftDown = 0x0002;

    /// <summary>
    /// Left mouse button up.
    /// </summary>
    internal const uint MouseEventFLeftUp = 0x0004;

    /// <summary>
    /// Right mouse button down.
    /// </summary>
    internal const uint MouseEventFRightDown = 0x0008;

    /// <summary>
    /// Right mouse button up.
    /// </summary>
    internal const uint MouseEventFRightUp = 0x0010;

    /// <summary>
    /// Middle mouse button down.
    /// </summary>
    internal const uint MouseEventFMiddleDown = 0x0020;

    /// <summary>
    /// Middle mouse button up.
    /// </summary>
    internal const uint MouseEventFMiddleUp = 0x0040;

    /// <summary>
    /// Mouse wheel event.
    /// </summary>
    internal const uint MouseEventFWheel = 0x0800;

    /// <summary>
    /// Standard Windows mouse wheel delta.
    /// </summary>
    internal const int WheelDelta = 120;

    /// <summary>
    /// Sends synthesized input events.
    /// </summary>
    /// <param name="nInputs">The number of structures in the input array.</param>
    /// <param name="pInputs">The input structures to send.</param>
    /// <param name="cbSize">The size, in bytes, of the <see cref="INPUT"/> structure.</param>
    /// <returns>The number of events successfully inserted into the input stream.</returns>
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    /// <summary>
    /// Native INPUT structure used by <see cref="SendInput"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct INPUT
    {
        /// <summary>
        /// The input type.
        /// </summary>
        public uint type;

        /// <summary>
        /// The union containing the actual input payload.
        /// </summary>
        public InputUnion U;
    }

    /// <summary>
    /// Native INPUT union.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct InputUnion
    {
        /// <summary>
        /// Mouse input payload.
        /// </summary>
        [FieldOffset(0)]
        public MOUSEINPUT mi;

        /// <summary>
        /// Keyboard input payload.
        /// </summary>
        [FieldOffset(0)]
        public KEYBDINPUT ki;
    }

    /// <summary>
    /// Native keyboard input structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct KEYBDINPUT
    {
        /// <summary>
        /// Virtual-key code.
        /// </summary>
        public ushort wVk;

        /// <summary>
        /// Hardware scan code.
        /// </summary>
        public ushort wScan;

        /// <summary>
        /// Flags specifying various aspects of the keystroke.
        /// </summary>
        public uint dwFlags;

        /// <summary>
        /// Event timestamp.
        /// </summary>
        public uint time;

        /// <summary>
        /// Additional application-defined information.
        /// </summary>
        public nint dwExtraInfo;
    }

    /// <summary>
    /// Native mouse input structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSEINPUT
    {
        /// <summary>
        /// Absolute or relative x coordinate.
        /// </summary>
        public int dx;

        /// <summary>
        /// Absolute or relative y coordinate.
        /// </summary>
        public int dy;

        /// <summary>
        /// Mouse wheel delta or button-specific data.
        /// </summary>
        public uint mouseData;

        /// <summary>
        /// Flags specifying various aspects of the mouse event.
        /// </summary>
        public uint dwFlags;

        /// <summary>
        /// Event timestamp.
        /// </summary>
        public uint time;

        /// <summary>
        /// Additional application-defined information.
        /// </summary>
        public nint dwExtraInfo;
    }
}