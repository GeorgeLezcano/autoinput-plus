using System.Diagnostics.CodeAnalysis;
using AutoInputPlus.Input.Windows.Mapping;

namespace AutoInputPlus.Input.Windows.Tests.Mapping;

/// <summary>
/// Tests for <see cref="KeyCodeMapper"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class KeyCodeMapperTests
{
    /// <summary>
    /// Try Map To Virtual Key When Letter Returns Expected Code
    /// </summary>
    [Theory]
    [InlineData("A", 0x41)]
    [InlineData("Z", 0x5A)]
    [InlineData("a", 0x41)]
    [InlineData("m", 0x4D)]
    public void TryMapToVirtualKeyWhenLetterReturnsExpectedCode(string key, ushort expected)
    {
        bool result = KeyCodeMapper.TryMapToVirtualKey(key, out ushort actual);

        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Try Map To Virtual Key When Digit Returns Expected Code
    /// </summary>
    [Theory]
    [InlineData("0", 0x30)]
    [InlineData("5", 0x35)]
    [InlineData("9", 0x39)]
    public void TryMapToVirtualKeyWhenDigitReturnsExpectedCode(string key, ushort expected)
    {
        bool result = KeyCodeMapper.TryMapToVirtualKey(key, out ushort actual);

        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Try Map To Virtual Key When Special Key Returns Expected Code
    /// </summary>
    [Theory]
    [InlineData("Enter", 0x0D)]
    [InlineData("Return", 0x0D)]
    [InlineData("Tab", 0x09)]
    [InlineData("Escape", 0x1B)]
    [InlineData("Esc", 0x1B)]
    [InlineData("Space", 0x20)]
    [InlineData("Backspace", 0x08)]
    [InlineData("Delete", 0x2E)]
    [InlineData("Del", 0x2E)]
    [InlineData("Insert", 0x2D)]
    [InlineData("Home", 0x24)]
    [InlineData("End", 0x23)]
    [InlineData("PageUp", 0x21)]
    [InlineData("PageDown", 0x22)]
    [InlineData("Left", 0x25)]
    [InlineData("Up", 0x26)]
    [InlineData("Right", 0x27)]
    [InlineData("Down", 0x28)]
    public void TryMapToVirtualKeyWhenSpecialKeyReturnsExpectedCode(string key, ushort expected)
    {
        bool result = KeyCodeMapper.TryMapToVirtualKey(key, out ushort actual);

        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Try Map To Virtual Key When Function Key Returns Expected Code
    /// </summary>
    [Theory]
    [InlineData("F1", 0x70)]
    [InlineData("F5", 0x74)]
    [InlineData("F12", 0x7B)]
    public void TryMapToVirtualKeyWhenFunctionKeyReturnsExpectedCode(string key, ushort expected)
    {
        bool result = KeyCodeMapper.TryMapToVirtualKey(key, out ushort actual);

        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Try Map To Virtual Key When Modifier Or Windows Key Returns Expected Code
    /// </summary>
    [Theory]
    [InlineData("Ctrl", 0x11)]
    [InlineData("Control", 0x11)]
    [InlineData("Shift", 0x10)]
    [InlineData("Alt", 0x12)]
    [InlineData("LeftCtrl", 0xA2)]
    [InlineData("RightCtrl", 0xA3)]
    [InlineData("LeftShift", 0xA0)]
    [InlineData("RightShift", 0xA1)]
    [InlineData("LeftAlt", 0xA4)]
    [InlineData("RightAlt", 0xA5)]
    [InlineData("Win", 0x5B)]
    [InlineData("Windows", 0x5B)]
    [InlineData("LWin", 0x5B)]
    [InlineData("RWin", 0x5C)]
    public void TryMapToVirtualKeyWhenModifierOrWindowsKeyReturnsExpectedCode(string key, ushort expected)
    {
        bool result = KeyCodeMapper.TryMapToVirtualKey(key, out ushort actual);

        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Try Map To Virtual Key When Numpad Key Returns Expected Code
    /// </summary>
    [Theory]
    [InlineData("NumPad0", 0x60)]
    [InlineData("NumPad5", 0x65)]
    [InlineData("NumPad9", 0x69)]
    [InlineData("Add", 0x6B)]
    [InlineData("Subtract", 0x6D)]
    [InlineData("Multiply", 0x6A)]
    [InlineData("Divide", 0x6F)]
    [InlineData("Decimal", 0x6E)]
    public void TryMapToVirtualKeyWhenNumpadKeyReturnsExpectedCode(string key, ushort expected)
    {
        bool result = KeyCodeMapper.TryMapToVirtualKey(key, out ushort actual);

        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Try Map To Virtual Key Trims And Ignores Case
    /// </summary>
    [Theory]
    [InlineData(" enter ", 0x0D)]
    [InlineData(" a ", 0x41)]
    [InlineData(" f8 ", 0x77)]
    public void TryMapToVirtualKeyTrimsAndIgnoresCase(string key, ushort expected)
    {
        bool result = KeyCodeMapper.TryMapToVirtualKey(key, out ushort actual);

        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Try Map To Virtual Key When Unsupported Returns False
    /// </summary>
    [Theory]
    [InlineData("NotARealKey")]
    [InlineData("F13")]
    [InlineData("MouseLeft")]
    [InlineData("??")]
    public void TryMapToVirtualKeyWhenUnsupportedReturnsFalse(string key)
    {
        bool result = KeyCodeMapper.TryMapToVirtualKey(key, out ushort actual);

        Assert.False(result);
        Assert.Equal((ushort)0, actual);
    }

    /// <summary>
    /// Try Map To Virtual Key When Key Is Null Throws ArgumentNullException
    /// </summary>
    [Fact]
    public void TryMapToVirtualKeyWhenKeyIsNullThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => KeyCodeMapper.TryMapToVirtualKey(null!, out _));
    }

    /// <summary>
    /// Try Map To Virtual Key When Key Is Whitespace Throws ArgumentException
    /// </summary>
    [Fact]
    public void TryMapToVirtualKeyWhenKeyIsWhitespaceThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => KeyCodeMapper.TryMapToVirtualKey("   ", out _));
    }
}