using AutoInputPlus.Core.Constants;

namespace AutoInputPlus.Core.Interfaces;

/// <summary>
/// Defines the core runtime engine responsible for executing
/// automated input operations.
/// </summary>
public interface IEngine
{
    /// <summary>
    /// Gets the current state of the engine.
    /// </summary>
    EngineState State { get; }

    /// <summary>
    /// Starts the engine and begins processing automated input.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous start operation.
    /// </returns>
    Task StartAsync();

    /// <summary>
    /// Stops the engine and halts any active automated input.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous stop operation.
    /// </returns>
    Task StopAsync();

    /// <summary>
    /// Toggles the engine between running and stopped states.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous toggle operation.
    /// </returns>
    Task ToggleAsync();
}