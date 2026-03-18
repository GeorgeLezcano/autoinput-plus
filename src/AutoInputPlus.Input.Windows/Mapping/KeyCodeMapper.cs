namespace AutoInputPlus.Input.Windows.Mapping;

/// <summary>
/// Maps application key tokens to Windows virtual-key codes.
/// </summary>
public static class KeyCodeMapper
{
    /// <summary>
    /// Attempts to map an application key token to a Windows virtual-key code.
    /// </summary>
    /// <param name="key">
    /// The application key token, such as <c>A</c>, <c>Enter</c>, or <c>F8</c>.
    /// </param>
    /// <param name="virtualKeyCode">
    /// When this method returns, contains the resolved Windows virtual-key code
    /// if the mapping succeeded; otherwise, <c>0</c>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the key token is supported; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryMapToVirtualKey(string key, out ushort virtualKeyCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        virtualKeyCode = 0;

        // TODO:
        // - Map letters A-Z
        // - Map digits 0-9
        // - Map function keys F1-F12
        // - Map special keys like Enter, Tab, Escape, Space
        // - Map arrows and modifier keys if needed by execution logic

        return false;
    }
}