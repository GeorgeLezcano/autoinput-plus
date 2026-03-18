using AutoInputPlus.Core.Enums;

namespace AutoInputPlus.Core.Interfaces;

/// <summary>
/// TODO
/// </summary>
public interface IInputSender
{
    /// <summary>
    /// TODO Add Xmls
    /// </summary>
    /// <param name="mouseInput"></param>
    void SendMouseInput(MOUSEINPUT mouseInput);

    /// <summary>
    /// TODO Add Xmls
    /// </summary>
    /// <param name="keyboardInput"></param>
    void SendKeyboardInput(KEYBOARDINPUT keyboardInput);

    //TODO Fix interface definition + additional methods if needed
}