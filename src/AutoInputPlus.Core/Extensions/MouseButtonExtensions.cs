using AutoInputPlus.Core.Enums;

namespace AutoInputPlus.Core.Extensions;

/// <summary>
/// Provides display helpers for <see cref="MouseButton"/> values.
/// </summary>
public static class MouseButtonExtensions
{
    /// <summary>
    /// Converts a <see cref="MouseButton"/> value to a user-friendly display name.
    /// </summary>
    /// <param name="button">The mouse button to format.</param>
    /// <returns>A user-friendly display name.</returns>
    public static string ToDisplayName(this MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => "Left Click",
            MouseButton.Right => "Right Click",
            MouseButton.Middle => "Middle Click",
            _ => button.ToString()
        };
    }
}