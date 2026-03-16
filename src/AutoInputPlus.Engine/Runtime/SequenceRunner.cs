using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Engine.Runtime;

/// <summary>
/// Defines operations for executing sequences of automated input steps.
/// </summary>
public sealed class SequenceRunner : ISequenceRunner
{
    /// <inheritdoc/>
    public Task ExecuteAsync(Sequence sequence)
    {
        throw new NotImplementedException();
    }
}