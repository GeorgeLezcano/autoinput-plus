using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Core.Interfaces;

/// <summary>
/// Defines operations for executing sequences of automated input steps.
/// </summary>
public interface ISequenceRunner
{
    /// <summary>
    /// Executes the specified sequence.
    /// </summary>
    /// <param name="sequence">
    /// The sequence to execute.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous execution operation.
    /// </returns>
    Task ExecuteAsync(Sequence sequence);
}