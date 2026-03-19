namespace AutoInputPlus.Core.Enums;

/// <summary>
/// Represents a supported keyboard key for the application.
/// This enum is the source of truth for all keyboard keys supported by the app.
/// If a key is not defined here, it is not supported.
/// </summary>
public enum InputKey
{
    // Letters
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z,

    // Top-row digits
    D0,
    D1,
    D2,
    D3,
    D4,
    D5,
    D6,
    D7,
    D8,
    D9,

    // Common control keys
    Backspace,
    Tab,
    Enter,
    Pause,
    CapsLock,
    Escape,
    Space,

    // Navigation
    PageUp,
    PageDown,
    End,
    Home,
    Left,
    Up,
    Right,
    Down,
    Insert,
    Delete,
    PrintScreen,

    // Windows keys
    LeftWin,
    RightWin,

    // Numpad
    NumPad0,
    NumPad1,
    NumPad2,
    NumPad3,
    NumPad4,
    NumPad5,
    NumPad6,
    NumPad7,
    NumPad8,
    NumPad9,
    Multiply,
    Add,
    Separator,
    Subtract,
#pragma warning disable CA1720 // Identifier contains type name
    Decimal,
#pragma warning restore CA1720 // Identifier contains type name
    Divide,

    // Function keys
    F1,
    F2,
    F3,
    F4,
    F5,
    F6,
    F7,
    F8,
    F9,
    F10,
    F11,
    F12,

    // Explicit modifiers as keys
    LeftShift,
    RightShift,
    LeftCtrl,
    RightCtrl,
    LeftAlt,
    RightAlt

    #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}