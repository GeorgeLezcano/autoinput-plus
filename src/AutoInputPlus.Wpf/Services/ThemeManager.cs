using System.Windows;
using AutoInputPlus.Core.Enums;
using Application = System.Windows.Application;

namespace AutoInputPlus.Wpf.Services;

/// <summary>
/// Helper class to manage application themes.
/// </summary>
public static class ThemeManager
{
    private const string ThemeFolder = "Themes/";

    /// <summary>
    /// Occurs when the current theme changes.
    /// </summary>
    public static event Action<AppTheme>? ThemeChanged;

    /// <summary>
    /// Gets the currently applied theme.
    /// </summary>
    public static AppTheme CurrentTheme { get; private set; } = AppTheme.LightBlue;

    /// <summary>
    /// Applies the requested theme.
    /// </summary>
    /// <param name="appTheme">Theme to apply.</param>
    public static void ApplyTheme(AppTheme appTheme)
    {
        string relativePath = appTheme switch
        {
            AppTheme.LightBlue => $"{ThemeFolder}LightBlueTheme.xaml",
            AppTheme.DarkBlue => $"{ThemeFolder}DarkBlueTheme.xaml",
            _ => throw new ArgumentOutOfRangeException(nameof(appTheme), appTheme, "Unsupported theme."),
        };

        var mergedDictionaries = Application.Current.Resources.MergedDictionaries;

        var themeDictionaries = mergedDictionaries
            .Where(d =>
                d.Source is not null &&
                d.Source.OriginalString.StartsWith(ThemeFolder, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var dictionary in themeDictionaries)
        {
            mergedDictionaries.Remove(dictionary);
        }

        mergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri(relativePath, UriKind.Relative)
        });

        CurrentTheme = appTheme;
        ThemeChanged?.Invoke(CurrentTheme);
    }
}