using AutoInputPlus.Core.Interfaces;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Engine.Profile;

/// <summary>
/// Autoinput implementation to manage the
/// active profile.
/// </summary>
public sealed class ProfileManager : IProfileManager
{
    /// <inheritdoc/>
    public InputProfile ActiveProfile { get; private set; } = DefaultProfile;

    /// <inheritdoc/>
    public void SetActiveProfile(InputProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        //TODO Validate fields, ranges to make sure its a valid profile.

        ActiveProfile = profile;
    }

    /// <summary>
    /// Resets all the profile data to its default values. 
    /// NOTE: This method do not persist unsaved profiles 
    /// nor delete existing ones.
    /// </summary>
    public void ResetProfileToDefaultValues() => SetActiveProfile(DefaultProfile);

    /// <summary>
    /// Retrieves a default application profile. This
    /// operation is READ-ONLY.
    /// </summary>
    private static InputProfile DefaultProfile => new() //TODO Move to constants when fully defined.
    {
        IntervalMilliseconds = 500,
        RunUntilStopActive = true,
        RunUntilSetCountActive = false,
        StopInputCount = 0,
        StartStopKeybind = "F8", //TOD Get from real keys
        TargetInputKey = "LMouse", //TOD Get from real keys
        ScheduleStartEnabled = false,
        ScheduleStartTime = DateTime.Now, //TODO consider format
        ScheduleStopEnabled = false,
        ScheduleStopTime = DateTime.Now, //TODO consider format
        SequenceModeActive = false,
        HoldTargetEnabled = false
    };
}