using System.Diagnostics.CodeAnalysis;
using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Core.Constants;

/// <summary>
/// Default application constants.
/// </summary>
[ExcludeFromCodeCoverage]
public static class AppConstants
{
    #region Default Values

    /// <summary>
    /// Default input profile display name.
    /// </summary>
    public const string DefaultInputProfileName = "New Profile";

    /// <summary>
    /// Default sequence display name.
    /// </summary>
    public const string DefaultSequenceName = "New Sequence";

    /// <summary>
    /// Default interval value between repeated inputs.
    /// </summary>
    public const int DefaultIntervalMilliseconds = 500;

    /// <summary>
    /// Default stop input count when count-based execution is enabled.
    /// </summary>
    public const int DefaultStopInputCount = 0;

    /// <summary>
    /// Default keyboard hotkey used to start or stop execution.
    /// </summary>
    public static readonly Hotkey DefaultStartStopHotkey = new(InputKey.F8);

    /// <summary>
    /// Default target input binding used for single-input execution mode.
    /// </summary>
    public static readonly InputBinding DefaultTargetInputBinding = InputBinding.FromMouseButton(MouseButton.Left);

    #endregion

    #region String Formats

    /// <summary>
    /// Default sequence step display name format.
    /// </summary>
    public const string DefaultSequenceStepNameFormat = "Step {0}";

    #endregion

    #region Sections .csproj

    /// <summary>
    /// File name for the build props of the solution.
    /// </summary>
    public const string DirectoryBuildPropsFileName = "Directory.Build.props";

    /// <summary>
    /// Default fallback value when metadata cannot be resolved.
    /// </summary>
    public const string MetadataFallback = "N/A";

    /// <summary>
    /// Name for the version property on the csproj file.
    /// </summary>
    public const string VersionPropName = "Version";

    /// <summary>
    /// Property group on the csproj file.
    /// </summary>
    public const string PropertyGroup = "PropertyGroup";

    #endregion
}