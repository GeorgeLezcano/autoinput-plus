using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Interfaces;

namespace AutoInputPlus.Engine.Runtime;

/// <summary>
/// AutoInput Implementation for the runtime engine 
/// responsible for executing operations.
/// </summary>
public sealed class AutoInputEngine : IEngine
{
    /// <inheritdoc/>
    public EngineState State { get; private set; }

    /// <inheritdoc/>
    public async Task StartAsync()
    {
        //TODO start logic. This starts the key/mouse inputs.

        State = EngineState.Running;
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task StopAsync()
    {
        //TODO stop logic. This stops the key/mouse inputs.

        State = EngineState.Ready;
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task ToggleAsync()
    {
        switch (State)
        {
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
        }
    }
}