using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Engine.Runtime;

/// <summary>
/// AutoInputPlus implementation for the runtime engine
/// responsible for executing automation operations.
/// </summary>
public sealed class AutoInputEngine(
    IProfileManager profileManager,
    ISequenceRunner sequenceRunner,
    IInputSender inputSender) : IEngine, IDisposable
{
    private readonly IProfileManager _profileManager = profileManager;
    private readonly ISequenceRunner _sequenceRunner = sequenceRunner;
    private readonly IInputSender _inputSender = inputSender;

    private readonly SemaphoreSlim _transitionLock = new(1, 1);

    private CancellationTokenSource? _executionCancellationTokenSource;
    private Task? _executionTask;
    private bool _disposed;

    /// <inheritdoc/>
    public event EventHandler? StateChanged;

    /// <inheritdoc/>
    public EngineState State { get; private set; } = EngineState.Disabled;

    /// <inheritdoc/>
    public async Task EnableAsync()
    {
        ThrowIfDisposed();

        await _transitionLock.WaitAsync();

        try
        {
            if (State == EngineState.Disabled)
            {
                SetState(EngineState.Ready);
            }
        }
        finally
        {
            _transitionLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task DisableAsync()
    {
        ThrowIfDisposed();

        Task? executionTaskToAwait = null;
        CancellationTokenSource? cancellationTokenSourceToDispose = null;

        await _transitionLock.WaitAsync();

        try
        {
            if (State == EngineState.Disabled)
            {
                return;
            }

            _executionCancellationTokenSource?.Cancel();

            executionTaskToAwait = _executionTask;
            cancellationTokenSourceToDispose = _executionCancellationTokenSource;

            _executionTask = null;
            _executionCancellationTokenSource = null;
            SetState(EngineState.Disabled);
        }
        finally
        {
            _transitionLock.Release();
        }

        if (executionTaskToAwait is not null)
        {
            try
            {
                await executionTaskToAwait;
            }
            catch (OperationCanceledException)
            {
                // Expected while disabling active execution.
            }
        }

        cancellationTokenSourceToDispose?.Dispose();
    }

    /// <inheritdoc/>
    public async Task StartAsync()
    {
        ThrowIfDisposed();

        await _transitionLock.WaitAsync();

        try
        {
            if (State != EngineState.Ready)
            {
                return;
            }

            CancellationTokenSource executionCancellationTokenSource = new();

            _executionCancellationTokenSource = executionCancellationTokenSource;
            _executionTask = RunExecutionLoopAsync(executionCancellationTokenSource.Token);

            SetState(EngineState.Running);
        }
        finally
        {
            _transitionLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task StopAsync()
    {
        ThrowIfDisposed();

        Task? executionTaskToAwait = null;
        CancellationTokenSource? cancellationTokenSourceToDispose = null;

        await _transitionLock.WaitAsync();

        try
        {
            if (State != EngineState.Running && State != EngineState.Scheduled)
            {
                return;
            }

            _executionCancellationTokenSource?.Cancel();

            executionTaskToAwait = _executionTask;
            cancellationTokenSourceToDispose = _executionCancellationTokenSource;

            _executionTask = null;
            _executionCancellationTokenSource = null;
        }
        finally
        {
            _transitionLock.Release();
        }

        if (executionTaskToAwait is not null)
        {
            try
            {
                await executionTaskToAwait;
            }
            catch (OperationCanceledException)
            {
                // Expected while stopping active execution.
            }
        }

        cancellationTokenSourceToDispose?.Dispose();

        await _transitionLock.WaitAsync();

        try
        {
            if (State != EngineState.Disabled && State != EngineState.Error)
            {
                SetState(EngineState.Ready);
            }
        }
        finally
        {
            _transitionLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task ToggleExecutionAsync()
    {
        ThrowIfDisposed();

        switch (State)
        {
            case EngineState.Disabled:
                return;

            case EngineState.Ready:
                await StartAsync();
                break;

            case EngineState.Running:
            case EngineState.Scheduled:
                await StopAsync();
                break;

            case EngineState.Error:
                return;
        }
    }

    private async Task RunExecutionLoopAsync(CancellationToken cancellationToken)
    {
        int completedExecutionCount = 0;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                InputProfile activeProfile = _profileManager.ActiveProfile;

                if (activeProfile.SequenceModeActive)
                {
                    bool executedSequence = await ExecuteSelectedSequenceAsync(activeProfile, cancellationToken);

                    if (!executedSequence)
                    {
                        break;
                    }

                    completedExecutionCount++;

                    if (ShouldStopAfterExecution(activeProfile, completedExecutionCount))
                    {
                        break;
                    }

                    continue;
                }

                if (activeProfile.HoldTargetEnabled)
                {
                    bool startedHold = await ExecuteHeldTargetUntilStoppedAsync(
                        activeProfile.TargetInputBinding,
                        cancellationToken);

                    if (!startedHold)
                    {
                        break;
                    }

                    break;
                }

                bool executedSingleTarget = ExecuteSingleTarget(activeProfile.TargetInputBinding);

                if (!executedSingleTarget)
                {
                    break;
                }

                completedExecutionCount++;

                if (ShouldStopAfterExecution(activeProfile, completedExecutionCount))
                {
                    break;
                }

                int intervalMilliseconds = Math.Max(0, activeProfile.IntervalMilliseconds);

                if (intervalMilliseconds > 0)
                {
                    await Task.Delay(intervalMilliseconds, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected while stopping execution.
        }
        catch
        {
            SetState(EngineState.Error);
            throw;
        }
        finally
        {
            if (State == EngineState.Running)
            {
                SetState(EngineState.Ready);
            }
        }
    }

    private async Task<bool> ExecuteSelectedSequenceAsync(
        InputProfile profile,
        CancellationToken cancellationToken)
    {
        if (profile.SelectedSequenceIndex < 0 || profile.SelectedSequenceIndex >= profile.Sequences.Count)
        {
            return false;
        }

        Sequence selectedSequence = profile.Sequences[profile.SelectedSequenceIndex];
        await _sequenceRunner.ExecuteAsync(selectedSequence, cancellationToken);

        return true;
    }

    private async Task<bool> ExecuteHeldTargetUntilStoppedAsync(
        InputBinding inputBinding,
        CancellationToken cancellationToken)
    {
        bool targetPressed = PressTargetDown(inputBinding);

        if (!targetPressed)
        {
            return false;
        }

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        finally
        {
            ReleaseTarget(inputBinding);
        }

        return true;
    }

    private bool ExecuteSingleTarget(InputBinding inputBinding)
    {
        if (inputBinding.IsKeyboard && inputBinding.Key.HasValue)
        {
            _inputSender.KeyPress(inputBinding.Key.Value);
            return true;
        }

        if (inputBinding.IsMouse && inputBinding.MouseButton.HasValue)
        {
            _inputSender.MouseClick(inputBinding.MouseButton.Value);
            return true;
        }

        return false;
    }

    private bool PressTargetDown(InputBinding inputBinding)
    {
        if (inputBinding.IsKeyboard && inputBinding.Key.HasValue)
        {
            _inputSender.KeyDown(inputBinding.Key.Value);
            return true;
        }

        if (inputBinding.IsMouse && inputBinding.MouseButton.HasValue)
        {
            _inputSender.MouseDown(inputBinding.MouseButton.Value);
            return true;
        }

        return false;
    }

    private void ReleaseTarget(InputBinding inputBinding)
    {
        if (inputBinding.IsKeyboard && inputBinding.Key.HasValue)
        {
            _inputSender.KeyUp(inputBinding.Key.Value);
            return;
        }

        if (inputBinding.IsMouse && inputBinding.MouseButton.HasValue)
        {
            _inputSender.MouseUp(inputBinding.MouseButton.Value);
        }
    }

    private static bool ShouldStopAfterExecution(InputProfile profile, int completedExecutionCount)
    {
        if (profile.RunUntilStopActive)
        {
            return false;
        }

        if (profile.RunUntilSetCountActive)
        {
            int stopInputCount = Math.Max(1, profile.StopInputCount);
            return completedExecutionCount >= stopInputCount;
        }

        return true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            _executionCancellationTokenSource?.Cancel();

            if (_executionTask is not null)
            {
                _executionTask.GetAwaiter().GetResult();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected while disposing during active execution.
        }
        finally
        {
            _executionCancellationTokenSource?.Dispose();
            _transitionLock.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    private void SetState(EngineState newState)
    {
        if (State == newState)
        {
            return;
        }

        State = newState;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}