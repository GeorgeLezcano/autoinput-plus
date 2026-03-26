namespace AutoInputPlus.Core.Interfaces;

/// <summary>
/// Manages whether the application starts automatically with Windows.
/// </summary>
public interface IStartupRegistrationService
{
    /// <summary>
    /// Gets a value indicating whether the application is registered to start with Windows.
    /// </summary>
    bool IsEnabled();

    /// <summary>
    /// Enables or disables startup with Windows.
    /// </summary>
    /// <param name="enabled">True to enable startup; otherwise false.</param>
    void SetEnabled(bool enabled);
}