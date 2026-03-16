namespace AutoInputPlus.Core.Constants;

/// <summary>
/// Represents the current execution state of the automation engine.
/// </summary>
public enum EngineState
{
    /// <summary>
    /// The engine is initialized and ready to start.
    /// </summary>
    Ready,

    /// <summary>
    /// The engine is actively executing automated input.
    /// </summary>
    Running,

    /// <summary>
    /// The engine is waiting for a scheduled start time.
    /// </summary>
    Scheduled,

    /// <summary>
    /// The engine encountered an error condition.
    /// </summary>
    Error
}