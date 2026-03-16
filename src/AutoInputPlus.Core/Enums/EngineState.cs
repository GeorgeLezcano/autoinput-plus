namespace AutoInputPlus.Core.Enums;

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
    /// The engine is temporarily paused and may be resumed.
    /// </summary>
    Paused,

    /// <summary>
    /// The engine is not currently running.
    /// </summary>
    Stopped,

    /// <summary>
    /// The engine encountered an error condition.
    /// </summary>
    Error
}