using AutoInputPlus.Core.Enums;
using WpfMouseButton = System.Windows.Input.MouseButton;
using WpfKey = System.Windows.Input.Key;

namespace AutoInputPlus.Wpf.Views;

/// <summary>
/// Shared WPF input capture mapping helpers.
/// </summary>
internal static class InputCaptureMapper
{
    public static bool TryMapToMouseButton(WpfMouseButton mouseButton, out MouseButton mappedButton)
    {
        switch (mouseButton)
        {
            case WpfMouseButton.Left:
                mappedButton = MouseButton.Left;
                return true;

            case WpfMouseButton.Right:
                mappedButton = MouseButton.Right;
                return true;

            case WpfMouseButton.Middle:
                mappedButton = MouseButton.Middle;
                return true;

            default:
                mappedButton = default;
                return false;
        }
    }

    public static bool TryMapToInputKey(WpfKey key, out InputKey inputKey)
    {
        switch (key)
        {
            case WpfKey.A: inputKey = InputKey.A; return true;
            case WpfKey.B: inputKey = InputKey.B; return true;
            case WpfKey.C: inputKey = InputKey.C; return true;
            case WpfKey.D: inputKey = InputKey.D; return true;
            case WpfKey.E: inputKey = InputKey.E; return true;
            case WpfKey.F: inputKey = InputKey.F; return true;
            case WpfKey.G: inputKey = InputKey.G; return true;
            case WpfKey.H: inputKey = InputKey.H; return true;
            case WpfKey.I: inputKey = InputKey.I; return true;
            case WpfKey.J: inputKey = InputKey.J; return true;
            case WpfKey.K: inputKey = InputKey.K; return true;
            case WpfKey.L: inputKey = InputKey.L; return true;
            case WpfKey.M: inputKey = InputKey.M; return true;
            case WpfKey.N: inputKey = InputKey.N; return true;
            case WpfKey.O: inputKey = InputKey.O; return true;
            case WpfKey.P: inputKey = InputKey.P; return true;
            case WpfKey.Q: inputKey = InputKey.Q; return true;
            case WpfKey.R: inputKey = InputKey.R; return true;
            case WpfKey.S: inputKey = InputKey.S; return true;
            case WpfKey.T: inputKey = InputKey.T; return true;
            case WpfKey.U: inputKey = InputKey.U; return true;
            case WpfKey.V: inputKey = InputKey.V; return true;
            case WpfKey.W: inputKey = InputKey.W; return true;
            case WpfKey.X: inputKey = InputKey.X; return true;
            case WpfKey.Y: inputKey = InputKey.Y; return true;
            case WpfKey.Z: inputKey = InputKey.Z; return true;
            case WpfKey.D0: inputKey = InputKey.D0; return true;
            case WpfKey.D1: inputKey = InputKey.D1; return true;
            case WpfKey.D2: inputKey = InputKey.D2; return true;
            case WpfKey.D3: inputKey = InputKey.D3; return true;
            case WpfKey.D4: inputKey = InputKey.D4; return true;
            case WpfKey.D5: inputKey = InputKey.D5; return true;
            case WpfKey.D6: inputKey = InputKey.D6; return true;
            case WpfKey.D7: inputKey = InputKey.D7; return true;
            case WpfKey.D8: inputKey = InputKey.D8; return true;
            case WpfKey.D9: inputKey = InputKey.D9; return true;
            case WpfKey.NumPad0: inputKey = InputKey.NumPad0; return true;
            case WpfKey.NumPad1: inputKey = InputKey.NumPad1; return true;
            case WpfKey.NumPad2: inputKey = InputKey.NumPad2; return true;
            case WpfKey.NumPad3: inputKey = InputKey.NumPad3; return true;
            case WpfKey.NumPad4: inputKey = InputKey.NumPad4; return true;
            case WpfKey.NumPad5: inputKey = InputKey.NumPad5; return true;
            case WpfKey.NumPad6: inputKey = InputKey.NumPad6; return true;
            case WpfKey.NumPad7: inputKey = InputKey.NumPad7; return true;
            case WpfKey.NumPad8: inputKey = InputKey.NumPad8; return true;
            case WpfKey.NumPad9: inputKey = InputKey.NumPad9; return true;
            case WpfKey.Multiply: inputKey = InputKey.Multiply; return true;
            case WpfKey.Add: inputKey = InputKey.Add; return true;
            case WpfKey.Separator: inputKey = InputKey.Separator; return true;
            case WpfKey.Subtract: inputKey = InputKey.Subtract; return true;
            case WpfKey.Decimal: inputKey = InputKey.Decimal; return true;
            case WpfKey.Divide: inputKey = InputKey.Divide; return true;
            case WpfKey.F1: inputKey = InputKey.F1; return true;
            case WpfKey.F2: inputKey = InputKey.F2; return true;
            case WpfKey.F3: inputKey = InputKey.F3; return true;
            case WpfKey.F4: inputKey = InputKey.F4; return true;
            case WpfKey.F5: inputKey = InputKey.F5; return true;
            case WpfKey.F6: inputKey = InputKey.F6; return true;
            case WpfKey.F7: inputKey = InputKey.F7; return true;
            case WpfKey.F8: inputKey = InputKey.F8; return true;
            case WpfKey.F9: inputKey = InputKey.F9; return true;
            case WpfKey.F10: inputKey = InputKey.F10; return true;
            case WpfKey.F11: inputKey = InputKey.F11; return true;
            case WpfKey.F12: inputKey = InputKey.F12; return true;
            case WpfKey.Back: inputKey = InputKey.Backspace; return true;
            case WpfKey.Tab: inputKey = InputKey.Tab; return true;
            case WpfKey.Enter: inputKey = InputKey.Enter; return true;
            case WpfKey.Pause: inputKey = InputKey.Pause; return true;
            case WpfKey.CapsLock: inputKey = InputKey.CapsLock; return true;
            case WpfKey.Escape: inputKey = InputKey.Escape; return true;
            case WpfKey.Space: inputKey = InputKey.Space; return true;
            case WpfKey.PageUp: inputKey = InputKey.PageUp; return true;
            case WpfKey.PageDown: inputKey = InputKey.PageDown; return true;
            case WpfKey.End: inputKey = InputKey.End; return true;
            case WpfKey.Home: inputKey = InputKey.Home; return true;
            case WpfKey.Left: inputKey = InputKey.Left; return true;
            case WpfKey.Up: inputKey = InputKey.Up; return true;
            case WpfKey.Right: inputKey = InputKey.Right; return true;
            case WpfKey.Down: inputKey = InputKey.Down; return true;
            case WpfKey.Insert: inputKey = InputKey.Insert; return true;
            case WpfKey.Delete: inputKey = InputKey.Delete; return true;
            case WpfKey.PrintScreen: inputKey = InputKey.PrintScreen; return true;
            case WpfKey.LWin: inputKey = InputKey.LeftWin; return true;
            case WpfKey.RWin: inputKey = InputKey.RightWin; return true;
            case WpfKey.LeftShift: inputKey = InputKey.LeftShift; return true;
            case WpfKey.RightShift: inputKey = InputKey.RightShift; return true;
            case WpfKey.LeftCtrl: inputKey = InputKey.LeftCtrl; return true;
            case WpfKey.RightCtrl: inputKey = InputKey.RightCtrl; return true;
            case WpfKey.LeftAlt: inputKey = InputKey.LeftAlt; return true;
            case WpfKey.RightAlt: inputKey = InputKey.RightAlt; return true;
            //case WpfKey.Oem1: inputKey = InputKey.Semicolon; return true;
            //case WpfKey.OemPlus: inputKey = InputKey.Equals; return true;
            //case WpfKey.OemComma: inputKey = InputKey.Comma; return true;
            //case WpfKey.OemMinus: inputKey = InputKey.Minus; return true;
            //case WpfKey.OemPeriod: inputKey = InputKey.Period; return true;
            //case WpfKey.Oem2: inputKey = InputKey.Slash; return true;
            //case WpfKey.Oem3: inputKey = InputKey.Backtick; return true;
            //case WpfKey.Oem4: inputKey = InputKey.LeftBracket; return true;
            //case WpfKey.Oem5: inputKey = InputKey.Backslash; return true;
            //case WpfKey.Oem6: inputKey = InputKey.RightBracket; return true;
            //case WpfKey.Oem7: inputKey = InputKey.Quote; return true;

            default:
                inputKey = default;
                return false;
        }
    }
}