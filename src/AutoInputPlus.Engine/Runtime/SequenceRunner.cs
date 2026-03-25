using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Engine.Runtime;

/// <summary>
/// Executes ordered input steps for sequence mode.
/// </summary>
public sealed class SequenceRunner(
    IProfileManager profileManager,
    IInputSender inputSender) : ISequenceRunner
{
    private readonly IProfileManager _profileManager = profileManager;
    private readonly IInputSender _inputSender = inputSender;

    /// <inheritdoc/>
    public async Task ExecuteAsync(Sequence sequence, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(sequence);

        if (!_profileManager.ActiveProfile.SequenceModeActive)
        {
            return;
        }

        foreach (SequenceStep step in sequence.Steps)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!step.IsEnabled)
            {
                continue;
            }

            await ExecuteStepAsync(step, cancellationToken);

            if (step.DelayAfterMilliseconds > 0)
            {
                await Task.Delay(step.DelayAfterMilliseconds, cancellationToken);
            }
        }
    }

    private async Task ExecuteStepAsync(SequenceStep step, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(step);

        switch (step.TargetType)
        {
            case SequenceStepTargetType.Keyboard:
                await ExecuteKeyboardStepAsync(step, cancellationToken);
                break;

            case SequenceStepTargetType.MouseButton:
                await ExecuteMouseButtonStepAsync(step, cancellationToken);
                break;

            case SequenceStepTargetType.MouseWheel:
                ExecuteMouseWheelStep(step);
                break;
        }
    }

    private async Task ExecuteKeyboardStepAsync(SequenceStep step, CancellationToken cancellationToken)
    {
        if (!step.Key.HasValue)
        {
            return;
        }

        InputKey key = step.Key.Value;

        if (step.IsHold)
        {
            _inputSender.KeyDown(key);

            try
            {
                if (step.DurationMilliseconds > 0)
                {
                    await Task.Delay(step.DurationMilliseconds, cancellationToken);
                }
            }
            finally
            {
                _inputSender.KeyUp(key);
            }

            return;
        }

        _inputSender.KeyPress(key);
    }

    private async Task ExecuteMouseButtonStepAsync(SequenceStep step, CancellationToken cancellationToken)
    {
        if (!step.MouseButton.HasValue)
        {
            return;
        }

        MouseButton button = step.MouseButton.Value;

        if (step.IsHold)
        {
            _inputSender.MouseDown(button);

            try
            {
                if (step.DurationMilliseconds > 0)
                {
                    await Task.Delay(step.DurationMilliseconds, cancellationToken);
                }
            }
            finally
            {
                _inputSender.MouseUp(button);
            }

            return;
        }

        _inputSender.MouseClick(button);
    }

    private void ExecuteMouseWheelStep(SequenceStep step)
    {
        if (step.IsHold)
        {
            return;
        }

        if (step.MouseWheelDelta == 0)
        {
            return;
        }

        _inputSender.MouseWheel(step.MouseWheelDelta);
    }
}