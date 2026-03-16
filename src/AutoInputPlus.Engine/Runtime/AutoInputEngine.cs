using AutoInputPlus.Core.Constants;
using AutoInputPlus.Core.Interfaces;

namespace AutoInputPlus.Engine.Runtime;

/// <summary>
/// AutoInput Implementation for the runtime engine 
/// responsible for executing operations.
/// </summary>
public sealed class AutoInputEngine : IEngine
{
    /// <inheritdoc/>
    public EngineState State => throw new NotImplementedException();

    /// <summary>
    /// Flag that indicates if the sequence mode is currently
    /// selected. Runs individual inputs otherwise.
    /// </summary>
    public bool SequenceMode { get; set; } = false;

    /// <inheritdoc/>
    public Task Start()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task Stop()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task Toggle()
    {
        throw new NotImplementedException();
    }
}