using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Interfaces;

namespace AutoInputPlus.Input.Windows.Input;

/// <summary>
/// TODO Add xmls. This is the implementation
/// </summary>
public sealed class InputSender : IInputSender
{
    /// <inheritdoc/>
    public void SendKeyboardInput(KEYBOARDINPUT keyboardInput)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SendMouseInput(MOUSEINPUT mouseInput)
    {
        throw new NotImplementedException();
    }
}