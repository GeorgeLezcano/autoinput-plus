using System.Reflection;
using System.Xml.Linq;
using AutoInputPlus.Core.Constants;

namespace AutoInputPlus.Core.Metadata;

/// <summary>
/// Provides access to application metadata.
/// Prefers assembly metadata when appropriate and falls back to Directory.Build.props when needed.
/// </summary>
public static class AppMetadata
{
    /// <summary>
    /// Gets the application version formatted for display.
    /// Prefers assembly metadata and falls back to the Version property in Directory.Build.props.
    /// </summary>
    /// <returns>The application version, or the configured fallback value if unavailable.</returns>
    public static string GetVersion()
        => GetVersion(AppContext.BaseDirectory);

    /// <summary>
    /// Gets an application metadata value by property name.
    /// Reads from Directory.Build.props.
    /// </summary>
    /// <param name="propertyName">The property name to retrieve, such as Authors, Company, Product, or RepositoryUrl.</param>
    /// <returns>The property value, or the configured fallback value if unavailable.</returns>
    public static string GetValue(string propertyName)
        => GetValue(propertyName, AppContext.BaseDirectory);

    /// <summary>
    /// Gets the application version formatted for display using the provided base directory.
    /// Intended for testing.
    /// </summary>
    /// <param name="baseDirectory">The starting directory used when searching for Directory.Build.props.</param>
    /// <param name="preferAssembly">
    /// True to prefer assembly metadata first; false to force reading from Directory.Build.props first.
    /// </param>
    /// <returns>The application version, or the configured fallback value if unavailable.</returns>
    internal static string GetVersion(string baseDirectory, bool preferAssembly = true)
    {
        string? version = null;

        if (preferAssembly)
        {
            version =
                GetAssemblyInformationalVersion()
                ?? GetAssemblyFileVersion()
                ?? GetAssemblyVersion();
        }

        version ??= ReadPropertyValue(AppConstants.VersionPropName, baseDirectory);

        return CleanVersion(version ?? AppConstants.MetadataFallback);
    }

    /// <summary>
    /// Gets an application metadata value by property name using the provided base directory.
    /// Intended for testing.
    /// </summary>
    /// <param name="propertyName">The property name to retrieve.</param>
    /// <param name="baseDirectory">The starting directory used when searching for Directory.Build.props.</param>
    /// <returns>The property value, or the configured fallback value if unavailable.</returns>
    internal static string GetValue(string propertyName, string baseDirectory)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return AppConstants.MetadataFallback;

        return ReadPropertyValue(propertyName, baseDirectory) ?? AppConstants.MetadataFallback;
    }

    private static string? GetAssemblyVersion()
    {
        try
        {
            return typeof(AppMetadata).Assembly.GetName().Version?.ToString();
        }
        catch
        {
            return null;
        }
    }

    private static string? GetAssemblyFileVersion()
    {
        return GetAssemblyAttribute<AssemblyFileVersionAttribute>(a => a.Version);
    }

    private static string? GetAssemblyInformationalVersion()
    {
        return GetAssemblyAttribute<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion);
    }

    private static string? GetAssemblyAttribute<T>(Func<T, string?> selector)
        where T : Attribute
    {
        try
        {
            var assembly = typeof(AppMetadata).Assembly;
            var attribute = assembly.GetCustomAttribute<T>();
            return attribute is null ? null : selector(attribute);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Reads a property value from Directory.Build.props by walking up the directory tree.
    /// </summary>
    /// <param name="propertyName">The property name to read.</param>
    /// <param name="baseDirectory">The starting directory used when searching for Directory.Build.props.</param>
    /// <returns>The property value if found; otherwise null.</returns>
    private static string? ReadPropertyValue(string propertyName, string baseDirectory)
    {
        try
        {
            var directory = new DirectoryInfo(baseDirectory);

            while (directory is not null)
            {
                var propsPath = Path.Combine(directory.FullName, AppConstants.DirectoryBuildPropsFileName);

                if (File.Exists(propsPath))
                {
                    var xml = XDocument.Load(propsPath);

                    var propertyGroups = xml.Root?.Elements(AppConstants.PropertyGroup);
                    if (propertyGroups is null)
                        return null;

                    foreach (var group in propertyGroups)
                    {
                        var element = group.Element(propertyName);
                        if (!string.IsNullOrWhiteSpace(element?.Value))
                            return element.Value.Trim();
                    }

                    return null;
                }

                directory = directory.Parent;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Cleans a semantic version string for display by removing build metadata and trimming a trailing revision of .0.
    /// </summary>
    /// <param name="version">The version string to clean.</param>
    /// <returns>The cleaned version string, or the configured fallback value if the input is invalid.</returns>
    private static string CleanVersion(string? version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return AppConstants.MetadataFallback;

        var plusIndex = version.IndexOf('+');
        if (plusIndex >= 0)
            version = version[..plusIndex];

        version = version.Trim();

        string? prerelease = null;
        var hyphenIndex = version.IndexOf('-');
        if (hyphenIndex >= 0)
        {
            prerelease = version[hyphenIndex..];
            version = version[..hyphenIndex];
        }

        var parts = version.Split('.', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 4 && parts[3] == "0")
            version = $"{parts[0]}.{parts[1]}.{parts[2]}";
        else
            version = string.Join('.', parts);

        return prerelease is not null ? version + prerelease : version;
    }
}