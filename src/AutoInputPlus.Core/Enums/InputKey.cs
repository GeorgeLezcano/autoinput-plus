namespace AutoInputPlus.Core.Enums;

/// <summary>
/// Represents a supported keyboard key for the application.
/// This enum is the source of truth for all keyboard keys supported by the app.
/// If a key is not defined here, it is not supported.
/// </summary>
public enum InputKey
{
    /// <summary>
    /// The A key.
    /// </summary>
    A,

    /// <summary>
    /// The B key.
    /// </summary>
    B,

    /// <summary>
    /// The C key.
    /// </summary>
    C,

    /// <summary>
    /// The D key.
    /// </summary>
    D,

    /// <summary>
    /// The E key.
    /// </summary>
    E,

    /// <summary>
    /// The F key.
    /// </summary>
    F,

    /// <summary>
    /// The G key.
    /// </summary>
    G,

    /// <summary>
    /// The H key.
    /// </summary>
    H,

    /// <summary>
    /// The I key.
    /// </summary>
    I,

    /// <summary>
    /// The J key.
    /// </summary>
    J,

    /// <summary>
    /// The K key.
    /// </summary>
    K,

    /// <summary>
    /// The L key.
    /// </summary>
    L,

    /// <summary>
    /// The M key.
    /// </summary>
    M,

    /// <summary>
    /// The N key.
    /// </summary>
    N,

    /// <summary>
    /// The O key.
    /// </summary>
    O,

    /// <summary>
    /// The P key.
    /// </summary>
    P,

    /// <summary>
    /// The Q key.
    /// </summary>
    Q,

    /// <summary>
    /// The R key.
    /// </summary>
    R,

    /// <summary>
    /// The S key.
    /// </summary>
    S,

    /// <summary>
    /// The T key.
    /// </summary>
    T,

    /// <summary>
    /// The U key.
    /// </summary>
    U,

    /// <summary>
    /// The V key.
    /// </summary>
    V,

    /// <summary>
    /// The W key.
    /// </summary>
    W,

    /// <summary>
    /// The X key.
    /// </summary>
    X,

    /// <summary>
    /// The Y key.
    /// </summary>
    Y,

    /// <summary>
    /// The Z key.
    /// </summary>
    Z,

    /// <summary>
    /// The 0 key on the top row of the keyboard.
    /// </summary>
    D0,

    /// <summary>
    /// The 1 key on the top row of the keyboard.
    /// </summary>
    D1,

    /// <summary>
    /// The 2 key on the top row of the keyboard.
    /// </summary>
    D2,

    /// <summary>
    /// The 3 key on the top row of the keyboard.
    /// </summary>
    D3,

    /// <summary>
    /// The 4 key on the top row of the keyboard.
    /// </summary>
    D4,

    /// <summary>
    /// The 5 key on the top row of the keyboard.
    /// </summary>
    D5,

    /// <summary>
    /// The 6 key on the top row of the keyboard.
    /// </summary>
    D6,

    /// <summary>
    /// The 7 key on the top row of the keyboard.
    /// </summary>
    D7,

    /// <summary>
    /// The 8 key on the top row of the keyboard.
    /// </summary>
    D8,

    /// <summary>
    /// The 9 key on the top row of the keyboard.
    /// </summary>
    D9,

    /// <summary>
    /// The Backspace key.
    /// </summary>
    Backspace,

    /// <summary>
    /// The Tab key.
    /// </summary>
    Tab,

    /// <summary>
    /// The Enter key.
    /// </summary>
    Enter,

    /// <summary>
    /// The Pause key.
    /// </summary>
    Pause,

    /// <summary>
    /// The Caps Lock key.
    /// </summary>
    CapsLock,

    /// <summary>
    /// The Escape key.
    /// </summary>
    Escape,

    /// <summary>
    /// The Space key.
    /// </summary>
    Space,

    /// <summary>
    /// The Page Up key.
    /// </summary>
    PageUp,

    /// <summary>
    /// The Page Down key.
    /// </summary>
    PageDown,

    /// <summary>
    /// The End key.
    /// </summary>
    End,

    /// <summary>
    /// The Home key.
    /// </summary>
    Home,

    /// <summary>
    /// The Left Arrow key.
    /// </summary>
    Left,

    /// <summary>
    /// The Up Arrow key.
    /// </summary>
    Up,

    /// <summary>
    /// The Right Arrow key.
    /// </summary>
    Right,

    /// <summary>
    /// The Down Arrow key.
    /// </summary>
    Down,

    /// <summary>
    /// The Insert key.
    /// </summary>
    Insert,

    /// <summary>
    /// The Delete key.
    /// </summary>
    Delete,

    /// <summary>
    /// The Print Screen key.
    /// </summary>
    PrintScreen,

    /// <summary>
    /// The left Windows key.
    /// </summary>
    LeftWin,

    /// <summary>
    /// The right Windows key.
    /// </summary>
    RightWin,

    /// <summary>
    /// The NumPad 0 key.
    /// </summary>
    NumPad0,

    /// <summary>
    /// The NumPad 1 key.
    /// </summary>
    NumPad1,

    /// <summary>
    /// The NumPad 2 key.
    /// </summary>
    NumPad2,

    /// <summary>
    /// The NumPad 3 key.
    /// </summary>
    NumPad3,

    /// <summary>
    /// The NumPad 4 key.
    /// </summary>
    NumPad4,

    /// <summary>
    /// The NumPad 5 key.
    /// </summary>
    NumPad5,

    /// <summary>
    /// The NumPad 6 key.
    /// </summary>
    NumPad6,

    /// <summary>
    /// The NumPad 7 key.
    /// </summary>
    NumPad7,

    /// <summary>
    /// The NumPad 8 key.
    /// </summary>
    NumPad8,

    /// <summary>
    /// The NumPad 9 key.
    /// </summary>
    NumPad9,

    /// <summary>
    /// The NumPad Multiply key.
    /// </summary>
    Multiply,

    /// <summary>
    /// The NumPad Add key.
    /// </summary>
    Add,

    /// <summary>
    /// The NumPad Separator key.
    /// </summary>
    Separator,

    /// <summary>
    /// The NumPad Subtract key.
    /// </summary>
    Subtract,

    /// <summary>
    /// The NumPad Decimal key.
    /// </summary>
#pragma warning disable CA1720
    Decimal,
#pragma warning restore CA1720

    /// <summary>
    /// The NumPad Divide key.
    /// </summary>
    Divide,

    /// <summary>
    /// The F1 key.
    /// </summary>
    F1,

    /// <summary>
    /// The F2 key.
    /// </summary>
    F2,

    /// <summary>
    /// The F3 key.
    /// </summary>
    F3,

    /// <summary>
    /// The F4 key.
    /// </summary>
    F4,

    /// <summary>
    /// The F5 key.
    /// </summary>
    F5,

    /// <summary>
    /// The F6 key.
    /// </summary>
    F6,

    /// <summary>
    /// The F7 key.
    /// </summary>
    F7,

    /// <summary>
    /// The F8 key.
    /// </summary>
    F8,

    /// <summary>
    /// The F9 key.
    /// </summary>
    F9,

    /// <summary>
    /// The F10 key.
    /// </summary>
    F10,

    /// <summary>
    /// The F11 key.
    /// </summary>
    F11,

    /// <summary>
    /// The F12 key.
    /// </summary>
    F12,

    /// <summary>
    /// The left Shift key.
    /// </summary>
    LeftShift,

    /// <summary>
    /// The right Shift key.
    /// </summary>
    RightShift,

    /// <summary>
    /// The left Control key.
    /// </summary>
    LeftCtrl,

    /// <summary>
    /// The right Control key.
    /// </summary>
    RightCtrl,

    /// <summary>
    /// The left Alt key.
    /// </summary>
    LeftAlt,

    /// <summary>
    /// The right Alt key.
    /// </summary>
    RightAlt
}