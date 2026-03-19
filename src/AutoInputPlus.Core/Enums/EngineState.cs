namespace AutoInputPlus.Core.Enums;

/// <summary>
/// Represents the current execution state of the automation engine.
/// </summary>
public enum EngineState
{
    /// <summary>
    /// The engine is disabled. The application may still be open,
    /// but execution hotkeys will not start automated input.
    /// </summary>
    Disabled,

    /// <summary>
    /// The engine is enabled and ready to start execution.
    /// </summary>
    Ready,

    /// <summary>
    /// The engine is actively executing automated input.
    /// </summary>
    Running,

    /// <summary>
    /// The engine is enabled and waiting for a scheduled start time.
    /// </summary>
    Scheduled,

    /// <summary>
    /// The engine encountered an error condition.
    /// </summary>
    Error
}