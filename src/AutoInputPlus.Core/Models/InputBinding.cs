using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Extensions;

namespace AutoInputPlus.Core.Models;

/// <summary>
/// Represents a single logical input binding that targets exactly one input source:
/// either a keyboard key or a mouse button.
/// </summary>
public sealed record InputBinding
{
    /// <summary>
    /// Gets the bound keyboard key, when the binding targets a keyboard input.
    /// </summary>
    public InputKey? Key { get; }

    /// <summary>
    /// Gets the bound mouse button, when the binding targets a mouse input.
    /// </summary>
    public MouseButton? MouseButton { get; }

    /// <summary>
    /// Gets a value indicating whether this binding targets a keyboard key.
    /// </summary>
    public bool IsKeyboard => Key.HasValue;

    /// <summary>
    /// Gets a value indicating whether this binding targets a mouse button.
    /// </summary>
    public bool IsMouse => MouseButton.HasValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="InputBinding"/> class.
    /// </summary>
    /// <param name="key">The keyboard key for the binding, if any.</param>
    /// <param name="mouseButton">The mouse button for the binding, if any.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the binding does not target exactly one input source.
    /// </exception>
    internal InputBinding(InputKey? key, MouseButton? mouseButton)
    {
        bool hasKey = key.HasValue;
        bool hasMouseButton = mouseButton.HasValue;

        if (hasKey == hasMouseButton)
        {
            throw new InvalidOperationException(
                "An input binding must target exactly one input source: either a keyboard key or a mouse button.");
        }

        Key = key;
        MouseButton = mouseButton;
    }

    /// <summary>
    /// Creates a binding for a keyboard key.
    /// </summary>
    /// <param name="key">The keyboard key to bind.</param>
    /// <returns>A keyboard input binding.</returns>
    public static InputBinding FromKey(InputKey key) => new(key, null);

    /// <summary>
    /// Creates a binding for a mouse button.
    /// </summary>
    /// <param name="mouseButton">The mouse button to bind.</param>
    /// <returns>A mouse input binding.</returns>
    public static InputBinding FromMouseButton(MouseButton mouseButton) => new(null, mouseButton);

    /// <summary>
    /// Returns a user-friendly display name for the binding.
    /// </summary>
    /// <returns>A display name for the binding.</returns>
    public string ToDisplayName()
    {
        return Key?.ToDisplayName() ?? MouseButton!.Value.ToDisplayName();
    }

    /// <summary>
    /// Returns a user-friendly display string for the binding.
    /// </summary>
    /// <returns>A user-friendly display string.</returns>
    public override string ToString() => ToDisplayName();
}