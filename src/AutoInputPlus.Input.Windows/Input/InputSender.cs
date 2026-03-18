using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Interfaces;

namespace AutoInputPlus.Input.Windows.Input;

/// <summary>
/// Windows implementation of <see cref="IInputSender"/>.
/// </summary>
/// <remarks>
/// This class will later translate application-level input requests into Win32
/// <c>SendInput</c> calls.
/// </remarks>
public sealed class InputSender : IInputSender
{
    /// <inheritdoc/>
    public void KeyPress(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void KeyDown(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void KeyUp(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void MouseClick(MouseButton button)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void MouseDown(MouseButton button)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void MouseUp(MouseButton button)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void MouseWheel(int delta)
    {
        throw new NotImplementedException();
    }
}