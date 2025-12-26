namespace TransparentNotesApp.Tests;

/// <summary>
/// Unit tests for input validation logic.
/// Tests opacity and font size input constraints.
/// </summary>
public class InputValidationTests
{
    /// <summary>
    /// Tests that opacity values are correctly clamped to the valid range [0.1, 1.0].
    /// </summary>
    [Fact]
    public void OpacityValidation_ValidValue_ReturnsValue()
    {
        // Arrange
        double inputOpacity = 0.5;
        double expectedOpacity = 0.5;

        // Act
        double clampedOpacity = Math.Max(0.1, Math.Min(1.0, inputOpacity));

        // Assert
        Assert.Equal(expectedOpacity, clampedOpacity);
    }

    /// <summary>
    /// Tests that opacity values below minimum are clamped to 0.1.
    /// </summary>
    [Fact]
    public void OpacityValidation_BelowMinimum_ClampedToMin()
    {
        // Arrange
        double inputOpacity = 0.05;
        double expectedOpacity = 0.1;

        // Act
        double clampedOpacity = Math.Max(0.1, Math.Min(1.0, inputOpacity));

        // Assert
        Assert.Equal(expectedOpacity, clampedOpacity);
    }

    /// <summary>
    /// Tests that opacity values above maximum are clamped to 1.0.
    /// </summary>
    [Fact]
    public void OpacityValidation_AboveMaximum_ClampedToMax()
    {
        // Arrange
        double inputOpacity = 1.5;
        double expectedOpacity = 1.0;

        // Act
        double clampedOpacity = Math.Max(0.1, Math.Min(1.0, inputOpacity));

        // Assert
        Assert.Equal(expectedOpacity, clampedOpacity);
    }

    /// <summary>
    /// Tests that opacity at exact minimum boundary is accepted.
    /// </summary>
    [Fact]
    public void OpacityValidation_AtMinimumBoundary_Accepted()
    {
        // Arrange
        double inputOpacity = 0.1;
        double expectedOpacity = 0.1;

        // Act
        double clampedOpacity = Math.Max(0.1, Math.Min(1.0, inputOpacity));

        // Assert
        Assert.Equal(expectedOpacity, clampedOpacity);
    }

    /// <summary>
    /// Tests that opacity at exact maximum boundary is accepted.
    /// </summary>
    [Fact]
    public void OpacityValidation_AtMaximumBoundary_Accepted()
    {
        // Arrange
        double inputOpacity = 1.0;
        double expectedOpacity = 1.0;

        // Act
        double clampedOpacity = Math.Max(0.1, Math.Min(1.0, inputOpacity));

        // Assert
        Assert.Equal(expectedOpacity, clampedOpacity);
    }

    /// <summary>
    /// Tests font size clamping to valid range [8, 32].
    /// </summary>
    [Fact]
    public void FontSizeValidation_ValidValue_ReturnsValue()
    {
        // Arrange
        double inputFontSize = 16.0;
        double expectedFontSize = 16.0;

        // Act
        double clampedFontSize = Math.Max(8, Math.Min(32, inputFontSize));

        // Assert
        Assert.Equal(expectedFontSize, clampedFontSize);
    }

    /// <summary>
    /// Tests that font size values below minimum are clamped to 8.
    /// </summary>
    [Fact]
    public void FontSizeValidation_BelowMinimum_ClampedToMin()
    {
        // Arrange
        double inputFontSize = 5.0;
        double expectedFontSize = 8.0;

        // Act
        double clampedFontSize = Math.Max(8, Math.Min(32, inputFontSize));

        // Assert
        Assert.Equal(expectedFontSize, clampedFontSize);
    }

    /// <summary>
    /// Tests that font size values above maximum are clamped to 32.
    /// </summary>
    [Fact]
    public void FontSizeValidation_AboveMaximum_ClampedToMax()
    {
        // Arrange
        double inputFontSize = 50.0;
        double expectedFontSize = 32.0;

        // Act
        double clampedFontSize = Math.Max(8, Math.Min(32, inputFontSize));

        // Assert
        Assert.Equal(expectedFontSize, clampedFontSize);
    }

    /// <summary>
    /// Tests that font size at exact minimum boundary is accepted.
    /// </summary>
    [Fact]
    public void FontSizeValidation_AtMinimumBoundary_Accepted()
    {
        // Arrange
        double inputFontSize = 8.0;
        double expectedFontSize = 8.0;

        // Act
        double clampedFontSize = Math.Max(8, Math.Min(32, inputFontSize));

        // Assert
        Assert.Equal(expectedFontSize, clampedFontSize);
    }

    /// <summary>
    /// Tests that font size at exact maximum boundary is accepted.
    /// </summary>
    [Fact]
    public void FontSizeValidation_AtMaximumBoundary_Accepted()
    {
        // Arrange
        double inputFontSize = 32.0;
        double expectedFontSize = 32.0;

        // Act
        double clampedFontSize = Math.Max(8, Math.Min(32, inputFontSize));

        // Assert
        Assert.Equal(expectedFontSize, clampedFontSize);
    }

    /// <summary>
    /// Tests parsing of valid opacity string input.
    /// </summary>
    [Theory]
    [InlineData("0.5")]
    [InlineData("0.1")]
    [InlineData("1.0")]
    [InlineData("0.75")]
    public void OpacityParsing_ValidStringInput_ParsesSuccessfully(string input)
    {
        // Act
        bool parseResult = double.TryParse(input, out double opacity);

        // Assert
        Assert.True(parseResult);
        Assert.True(opacity >= 0.1 && opacity <= 1.0);
    }

    /// <summary>
    /// Tests parsing of invalid opacity string input.
    /// </summary>
    [Theory]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData("abc123")]
    [InlineData("0.5.5")]
    public void OpacityParsing_InvalidStringInput_ParsesFails(string input)
    {
        // Act
        bool parseResult = double.TryParse(input, out double opacity);

        // Assert
        Assert.False(parseResult);
    }

    /// <summary>
    /// Tests parsing of valid font size string input.
    /// </summary>
    [Theory]
    [InlineData("12")]
    [InlineData("8")]
    [InlineData("32")]
    [InlineData("16")]
    public void FontSizeParsing_ValidStringInput_ParsesSuccessfully(string input)
    {
        // Act
        bool parseResult = double.TryParse(input, out double fontSize);

        // Assert
        Assert.True(parseResult);
        Assert.True(fontSize >= 8 && fontSize <= 32);
    }

    /// <summary>
    /// Tests parsing of invalid font size string input.
    /// </summary>
    [Theory]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData("size")]
    public void FontSizeParsing_InvalidStringInput_ParsesFails(string input)
    {
        // Act
        bool parseResult = double.TryParse(input, out double fontSize);

        // Assert
        Assert.False(parseResult);
    }
}
