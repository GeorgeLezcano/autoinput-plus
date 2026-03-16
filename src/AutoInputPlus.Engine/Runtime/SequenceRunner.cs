using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Engine.Runtime;

/// <summary>
/// Defines operations for executing sequences of automated input steps.
/// </summary>
/// <param name="profileManager"></param>
public sealed class SequenceRunner(IProfileManager profileManager) : ISequenceRunner
{
    private readonly IProfileManager _profileManager = profileManager;

    /// <inheritdoc/>
    public async Task ExecuteAsync(Sequence sequence)
    {
        ArgumentNullException.ThrowIfNull(sequence);

        if (!_profileManager.ActiveProfile.SequenceModeActive)
            return;

        // TODO Sequence execution logic. It should only execute if sequence mode is
        // currently active.

        await Task.CompletedTask;
    }
}