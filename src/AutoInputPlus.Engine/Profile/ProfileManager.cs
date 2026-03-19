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
    public InputProfile ActiveProfile { get; private set; } = DefaultProfile;

    /// <inheritdoc/>
    public void SetActiveProfile(InputProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ActiveProfile = profile;
    }

    /// <summary>
    /// Resets all the profile data to its default values.
    /// NOTE: This method does not persist unsaved profiles
    /// nor delete existing ones.
    /// </summary>
    public void ResetProfileToDefaultValues() => SetActiveProfile(DefaultProfile);

    /// <summary>
    /// Retrieves a default application profile.
    /// This operation is read-only.
    /// </summary>
    private static InputProfile DefaultProfile => new()
    {
        IntervalMilliseconds = AppConstants.DefaultIntervalMilliseconds,
        RunUntilStopActive = true,
        RunUntilSetCountActive = false,
        StopInputCount = AppConstants.DefaultStopInputCount,
        StartStopHotkey = AppConstants.DefaultStartStopHotkey,
        TargetInputBinding = AppConstants.DefaultTargetInputBinding,
        ScheduleStartEnabled = false,
        ScheduleStartTime = DateTime.Now,
        ScheduleStopEnabled = false,
        ScheduleStopTime = DateTime.Now,
        SequenceModeActive = false,
        HoldTargetEnabled = false
    };
}