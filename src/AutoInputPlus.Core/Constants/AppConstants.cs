namespace AutoInputPlus.Core.Constants;

/// <summary>
/// Default application constants.
/// </summary>
public static class AppConstants
{
    #region Default Names

    /// <summary>
    /// Default Input profile display name.
    /// </summary>
    public const string DefaultInputProfileName = "New Profile";

    /// <summary>
    /// Default Sequence display name.
    /// </summary>
    public const string DefaultSequenceName = "New Sequence";

    #endregion

    #region Sections .csproj

    /// <summary>
    /// File name for the Build props of the solution.
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