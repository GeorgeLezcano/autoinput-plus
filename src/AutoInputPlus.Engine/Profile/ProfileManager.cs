using AutoInputPlus.Core.Constants;
using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Engine.Profile;

/// <summary>
/// AutoInputPlus implementation to manage the active profile.
/// </summary>
public sealed class ProfileManager : IProfileManager
{
    /// <inheritdoc/>
    public InputProfile ActiveProfile { get; private set; } = CreateDefaultProfile();

    /// <inheritdoc/>
    public void SetActiveProfile(InputProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ActiveProfile = profile;
    }

    /// <summary>
    /// Creates a default application profile.
    /// </summary>
    /// <returns>
    /// A new <see cref="InputProfile"/> initialized with application defaults.
    /// </returns>
    public static InputProfile CreateDefaultProfile()
    {
        return new InputProfile
        {
            Name = AppConstants.DefaultInputProfileName,
            IntervalMilliseconds = AppConstants.DefaultIntervalMilliseconds,
            RunUntilStopActive = true,
            RunUntilSetCountActive = false,
            StopInputCount = AppConstants.DefaultStopInputCount,
            HoldTargetEnabled = false,
            StartStopHotkey = new Hotkey(InputKey.F8), // TODO Move to constants
            TargetInputBinding = InputBinding.FromMouseButton(MouseButton.Left), //TODO Move to constants
            ScheduleStartEnabled = false,
            ScheduleStartTime = DateTime.Now,
            ScheduleStopEnabled = false,
            ScheduleStopTime = DateTime.Now,
            SelectedSequenceIndex = 0,
            SequenceModeActive = false,
            Sequences = []
        };
    }
}