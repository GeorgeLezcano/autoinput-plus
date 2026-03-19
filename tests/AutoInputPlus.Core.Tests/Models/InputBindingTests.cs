using System.Diagnostics.CodeAnalysis;
using AutoInputPlus.Core.Enums;
using AutoInputPlus.Core.Models;

namespace AutoInputPlus.Core.Tests.Models;

/// <summary>
/// Tests for Hotkey class.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class InputBindingTests
{
    /// <summary>
    /// InputBindingInitializedFromKeyboard
    /// </summary>
    [Fact]
    public void InputBindingInitializedFromKeyboard()
    {
        var binding = InputBinding.FromKey(InputKey.C);

        Assert.Equal("C", binding.ToDisplayName());
        Assert.Equal("C", binding.ToString());

        Assert.True(binding.IsKeyboard);
        Assert.False(binding.IsMouse);
    }

    /// <summary>
    /// InputBindingInitializedFromMouse
    /// </summary>
    [Fact]
    public void InputBindingInitializedFromMouse()
    {
         var binding = InputBinding.FromMouseButton(MouseButton.Left);

        Assert.Equal("Left Click", binding.ToDisplayName());

        Assert.False(binding.IsKeyboard);
        Assert.True(binding.IsMouse);
    }

    /// <summary>
    /// InputBindingInitializedWithBothTypes
    /// </summary>
    [Fact]
    public void InputBindingInitializedWithBothTypes()
    {
        Assert.Throws<InvalidOperationException>(() => new InputBinding(InputKey.Z, MouseButton.Right));
    }

}