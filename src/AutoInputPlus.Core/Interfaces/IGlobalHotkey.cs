namespace AutoInputPlus.Core.Interfaces;

/// <summary>
/// TODO
/// </summary>
public interface IGlobalHotkey //TODO Any other methods on the interface? Also fix definition as needed
{
    /// <summary>
    /// TODO Add xmls
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool RegisterHotKey(string key); //Should this be a string? Is there a better type? Should it return bool?

    /// <summary>
    /// TODO Add xmls
    /// </summary>
    /// <returns></returns>
    bool UnregisterHotKey(); //TODO Should it return bool?
}
