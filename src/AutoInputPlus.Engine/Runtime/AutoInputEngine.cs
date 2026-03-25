using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Interfaces;

namespace AutoInputPlus.Engine.Runtime;

/// <summary>
/// AutoInputPlus implementation for the runtime engine
/// responsible for executing automation operations.
/// </summary>
public sealed class AutoInputEngine : IEngine
{
    /// <inheritdoc/>
    public EngineState State { get; private set; } = EngineState.Disabled;

    /// <inheritdoc/>
    public async Task EnableAsync()
    {
        if (State == EngineState.Disabled)
        {
            State = EngineState.Ready;
        }

        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task DisableAsync()
    {
        if (State == EngineState.Running || State == EngineState.Scheduled)
        {
            // TODO stop active execution or cancel scheduled work before disabling.
        }

        State = EngineState.Disabled;
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task StartAsync()
    {
        if (State != EngineState.Ready)
        {
            return;
        }

        // TODO start key/mouse input logic.

        State = EngineState.Running;
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task StopAsync()
    {
        if (State == EngineState.Disabled)
        {
            return;
        }

        if (State == EngineState.Running || State == EngineState.Scheduled)
        {
            // TODO stop key/mouse input logic.
            State = EngineState.Ready;
        }

        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task ToggleExecutionAsync()
    {
        switch (State)
        {
            case EngineState.Disabled:
                {
                    // TODO optionally log or notify that the engine is disabled.
                    return;
                }

            case EngineState.Ready:
                {
                    await StartAsync();
                    break;
                }

            case EngineState.Running:
                {
                    await StopAsync();
                    break;
                }

            case EngineState.Scheduled:
                {
                    await StopAsync();
                    break;
                }

            case EngineState.Error:
                {
                    // TODO optionally log or notify that the engine is in an error state.
                    return;
                }
        }
    }
}