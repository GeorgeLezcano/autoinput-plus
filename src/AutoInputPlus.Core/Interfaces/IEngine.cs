using AutoInputPlus.Core.Enums;

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
    /// Enables the engine and allows execution hotkeys to control input.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous enable operation.
    /// </returns>
    Task EnableAsync();

    /// <summary>
    /// Disables the engine and prevents execution hotkeys from starting input.
    /// Any active execution is stopped.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous disable operation.
    /// </returns>
    Task DisableAsync();

    /// <summary>
    /// Starts automated input execution.
    /// The engine must be enabled and ready before execution can begin.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous start operation.
    /// </returns>
    Task StartAsync();

    /// <summary>
    /// Stops active automated input execution and returns the engine
    /// to the ready state when the engine remains enabled.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous stop operation.
    /// </returns>
    Task StopAsync();

    /// <summary>
    /// Toggles automated input execution when the engine is enabled.
    /// If the engine is disabled, the request is ignored.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous toggle operation.
    /// </returns>
    Task ToggleExecutionAsync();
}