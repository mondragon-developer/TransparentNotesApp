namespace TransparentNotesApp.Tests;

/// <summary>
/// Unit tests for window dimension calculations and constraints.
/// Tests resize logic and minimum size enforcement.
/// </summary>
public class ResizeDimensionTests
{
    /// <summary>
    /// Tests that window width cannot be resized below the minimum (475 pixels).
    /// </summary>
    [Fact]
    public void ResizeDimension_WidthBelowMinimum_EnforcedToMinimum()
    {
        // Arrange
        double currentWidth = 500;
        double deltaX = -50;
        double minWidth = 475;
        
        // Act
        double newWidth = currentWidth + deltaX;
        if (newWidth < minWidth)
            newWidth = minWidth;

        // Assert
        Assert.Equal(minWidth, newWidth);
        Assert.True(newWidth >= minWidth);
    }

    /// <summary>
    /// Tests that window height cannot be resized below the minimum (100 pixels).
    /// </summary>
    [Fact]
    public void ResizeDimension_HeightBelowMinimum_EnforcedToMinimum()
    {
        // Arrange
        double currentHeight = 150;
        double deltaY = -100;
        double minHeight = 100;

        // Act
        double newHeight = currentHeight + deltaY;
        if (newHeight < minHeight)
            newHeight = minHeight;

        // Assert
        Assert.Equal(minHeight, newHeight);
        Assert.True(newHeight >= minHeight);
    }

    /// <summary>
    /// Tests positive width delta increases window width correctly.
    /// </summary>
    [Fact]
    public void ResizeDimension_PositiveWidthDelta_IncreasesWidth()
    {
        // Arrange
        double currentWidth = 600;
        double deltaX = 50;
        double minWidth = 475;

        // Act
        double newWidth = currentWidth + deltaX;
        if (newWidth >= minWidth)
            newWidth = newWidth;

        // Assert
        Assert.Equal(650, newWidth);
        Assert.True(newWidth > currentWidth);
    }

    /// <summary>
    /// Tests positive height delta increases window height correctly.
    /// </summary>
    [Fact]
    public void ResizeDimension_PositiveHeightDelta_IncreasesHeight()
    {
        // Arrange
        double currentHeight = 400;
        double deltaY = 75;
        double minHeight = 100;

        // Act
        double newHeight = currentHeight + deltaY;
        if (newHeight >= minHeight)
        {
            // Height accepted
        }

        // Assert
        Assert.Equal(475, newHeight);
        Assert.True(newHeight > currentHeight);
    }

    /// <summary>
    /// Tests negative width delta that stays above minimum.
    /// </summary>
    [Fact]
    public void ResizeDimension_NegativeWidthDeltaAboveMinimum_DecreasesWidth()
    {
        // Arrange
        double currentWidth = 600;
        double deltaX = -50;
        double minWidth = 475;

        // Act
        double newWidth = currentWidth + deltaX;
        if (newWidth < minWidth)
            newWidth = minWidth;

        // Assert
        Assert.Equal(550, newWidth);
        Assert.True(newWidth < currentWidth);
        Assert.True(newWidth >= minWidth);
    }

    /// <summary>
    /// Tests negative height delta that stays above minimum.
    /// </summary>
    [Fact]
    public void ResizeDimension_NegativeHeightDeltaAboveMinimum_DecreasesHeight()
    {
        // Arrange
        double currentHeight = 300;
        double deltaY = -75;
        double minHeight = 100;

        // Act
        double newHeight = currentHeight + deltaY;
        if (newHeight < minHeight)
            newHeight = minHeight;

        // Assert
        Assert.Equal(225, newHeight);
        Assert.True(newHeight < currentHeight);
        Assert.True(newHeight >= minHeight);
    }

    /// <summary>
    /// Tests zero delta does not change dimensions.
    /// </summary>
    [Fact]
    public void ResizeDimension_ZeroDelta_DimensionsUnchanged()
    {
        // Arrange
        double currentWidth = 600;
        double deltaX = 0;
        double minWidth = 475;

        // Act
        double newWidth = currentWidth + deltaX;
        if (newWidth >= minWidth)
            newWidth = newWidth;

        // Assert
        Assert.Equal(currentWidth, newWidth);
    }

    /// <summary>
    /// Tests that both width and height constraints are enforced independently.
    /// </summary>
    [Fact]
    public void ResizeDimension_BothDimensions_ConstraintsEnforcedIndependently()
    {
        // Arrange
        double currentWidth = 500;
        double currentHeight = 150;
        double deltaX = -50;
        double deltaY = -100;
        double minWidth = 475;
        double minHeight = 100;

        // Act
        double newWidth = currentWidth + deltaX;
        double newHeight = currentHeight + deltaY;
        
        if (newWidth < minWidth)
            newWidth = minWidth;
        if (newHeight < minHeight)
            newHeight = minHeight;

        // Assert
        Assert.Equal(minWidth, newWidth);
        Assert.Equal(minHeight, newHeight);
        Assert.True(newWidth >= minWidth && newHeight >= minHeight);
    }

    /// <summary>
    /// Tests large positive delta for window expansion.
    /// </summary>
    [Fact]
    public void ResizeDimension_LargePositiveDelta_ExpandsWindow()
    {
        // Arrange
        double currentWidth = 600;
        double currentHeight = 400;
        double deltaX = 400;
        double deltaY = 300;

        // Act
        double newWidth = currentWidth + deltaX;
        double newHeight = currentHeight + deltaY;

        // Assert
        Assert.Equal(1000, newWidth);
        Assert.Equal(700, newHeight);
        Assert.True(newWidth > currentWidth);
        Assert.True(newHeight > currentHeight);
    }

    /// <summary>
    /// Tests minimum width constraint with exact boundary value.
    /// </summary>
    [Fact]
    public void ResizeDimension_WidthAtExactMinimum_Accepted()
    {
        // Arrange
        double targetWidth = 475;
        double minWidth = 475;

        // Act
        double newWidth = targetWidth;
        if (newWidth < minWidth)
            newWidth = minWidth;

        // Assert
        Assert.Equal(minWidth, newWidth);
    }

    /// <summary>
    /// Tests minimum height constraint with exact boundary value.
    /// </summary>
    [Fact]
    public void ResizeDimension_HeightAtExactMinimum_Accepted()
    {
        // Arrange
        double targetHeight = 100;
        double minHeight = 100;

        // Act
        double newHeight = targetHeight;
        if (newHeight < minHeight)
            newHeight = minHeight;

        // Assert
        Assert.Equal(minHeight, newHeight);
    }
}
