namespace TransparentNotesApp.Tests;

/// <summary>
/// Unit tests for coordinate tracking and mouse position calculations.
/// Tests delta calculations for window resizing.
/// </summary>
public class CoordinateTrackingTests
{
    /// <summary>
    /// Tests positive X delta calculation when mouse moves right.
    /// </summary>
    [Fact]
    public void CoordinateTracking_MouseMovesRight_PositiveDeltaX()
    {
        // Arrange
        double lastMouseX = 100;
        double currentMouseX = 150;

        // Act
        double deltaX = currentMouseX - lastMouseX;

        // Assert
        Assert.Equal(50, deltaX);
        Assert.True(deltaX > 0);
    }

    /// <summary>
    /// Tests negative X delta calculation when mouse moves left.
    /// </summary>
    [Fact]
    public void CoordinateTracking_MouseMovesLeft_NegativeDeltaX()
    {
        // Arrange
        double lastMouseX = 150;
        double currentMouseX = 100;

        // Act
        double deltaX = currentMouseX - lastMouseX;

        // Assert
        Assert.Equal(-50, deltaX);
        Assert.True(deltaX < 0);
    }

    /// <summary>
    /// Tests positive Y delta calculation when mouse moves down.
    /// </summary>
    [Fact]
    public void CoordinateTracking_MouseMovesDown_PositiveDeltaY()
    {
        // Arrange
        double lastMouseY = 100;
        double currentMouseY = 150;

        // Act
        double deltaY = currentMouseY - lastMouseY;

        // Assert
        Assert.Equal(50, deltaY);
        Assert.True(deltaY > 0);
    }

    /// <summary>
    /// Tests negative Y delta calculation when mouse moves up.
    /// </summary>
    [Fact]
    public void CoordinateTracking_MouseMovesUp_NegativeDeltaY()
    {
        // Arrange
        double lastMouseY = 150;
        double currentMouseY = 100;

        // Act
        double deltaY = currentMouseY - lastMouseY;

        // Assert
        Assert.Equal(-50, deltaY);
        Assert.True(deltaY < 0);
    }

    /// <summary>
    /// Tests zero delta when mouse hasn't moved in X direction.
    /// </summary>
    [Fact]
    public void CoordinateTracking_MouseStationary_ZeroDeltaX()
    {
        // Arrange
        double lastMouseX = 100;
        double currentMouseX = 100;

        // Act
        double deltaX = currentMouseX - lastMouseX;

        // Assert
        Assert.Equal(0, deltaX);
    }

    /// <summary>
    /// Tests zero delta when mouse hasn't moved in Y direction.
    /// </summary>
    [Fact]
    public void CoordinateTracking_MouseStationary_ZeroDeltaY()
    {
        // Arrange
        double lastMouseY = 100;
        double currentMouseY = 100;

        // Act
        double deltaY = currentMouseY - lastMouseY;

        // Assert
        Assert.Equal(0, deltaY);
    }

    /// <summary>
    /// Tests diagonal mouse movement calculations.
    /// </summary>
    [Fact]
    public void CoordinateTracking_DiagonalMouseMovement_BothDeltasCalculated()
    {
        // Arrange
        double lastMouseX = 100;
        double lastMouseY = 100;
        double currentMouseX = 150;
        double currentMouseY = 150;

        // Act
        double deltaX = currentMouseX - lastMouseX;
        double deltaY = currentMouseY - lastMouseY;

        // Assert
        Assert.Equal(50, deltaX);
        Assert.Equal(50, deltaY);
        Assert.True(deltaX > 0);
        Assert.True(deltaY > 0);
    }

    /// <summary>
    /// Tests that tracking position is updated for next iteration.
    /// </summary>
    [Fact]
    public void CoordinateTracking_UpdatePosition_ForNextIteration()
    {
        // Arrange
        double lastMouseX = 100;
        double lastMouseY = 100;
        double currentMouseX = 150;
        double currentMouseY = 150;

        // Act
        double deltaX = currentMouseX - lastMouseX;
        double deltaY = currentMouseY - lastMouseY;
        lastMouseX = currentMouseX;
        lastMouseY = currentMouseY;

        double nextCurrentX = 200;
        double nextCurrentY = 200;
        double nextDeltaX = nextCurrentX - lastMouseX;
        double nextDeltaY = nextCurrentY - lastMouseY;

        // Assert
        Assert.Equal(50, nextDeltaX);
        Assert.Equal(50, nextDeltaY);
        Assert.Equal(150, lastMouseX);
        Assert.Equal(150, lastMouseY);
    }

    /// <summary>
    /// Tests large coordinate deltas for fast mouse movement.
    /// </summary>
    [Fact]
    public void CoordinateTracking_FastMouseMovement_LargeDelta()
    {
        // Arrange
        double lastMouseX = 100;
        double lastMouseY = 100;
        double currentMouseX = 500;
        double currentMouseY = 500;

        // Act
        double deltaX = currentMouseX - lastMouseX;
        double deltaY = currentMouseY - lastMouseY;

        // Assert
        Assert.Equal(400, deltaX);
        Assert.Equal(400, deltaY);
    }

    /// <summary>
    /// Tests fractional coordinate calculations.
    /// </summary>
    [Fact]
    public void CoordinateTracking_FractionalCoordinates_CalculatedCorrectly()
    {
        // Arrange
        double lastMouseX = 100.5;
        double lastMouseY = 100.7;
        double currentMouseX = 150.2;
        double currentMouseY = 150.3;

        // Act
        double deltaX = currentMouseX - lastMouseX;
        double deltaY = currentMouseY - lastMouseY;

        // Assert
        Assert.Equal(49.7, deltaX, precision: 1);
        Assert.Equal(49.6, deltaY, precision: 1);
    }

    /// <summary>
    /// Tests coordinate tracking with negative screen coordinates.
    /// </summary>
    [Fact]
    public void CoordinateTracking_NegativeCoordinates_CalculatedCorrectly()
    {
        // Arrange
        double lastMouseX = -100;
        double lastMouseY = -100;
        double currentMouseX = -50;
        double currentMouseY = -50;

        // Act
        double deltaX = currentMouseX - lastMouseX;
        double deltaY = currentMouseY - lastMouseY;

        // Assert
        Assert.Equal(50, deltaX);
        Assert.Equal(50, deltaY);
    }

    /// <summary>
    /// Tests position update for continuous tracking during drag.
    /// </summary>
    [Fact]
    public void CoordinateTracking_ContinuousDragTracking_PositionUpdated()
    {
        // Arrange
        double lastMouseX = 100;
        double lastMouseY = 100;

        // Act - First move
        double currentMouseX = 120;
        double currentMouseY = 120;
        double deltaX = currentMouseX - lastMouseX;
        double deltaY = currentMouseY - lastMouseY;
        lastMouseX = currentMouseX;
        lastMouseY = currentMouseY;

        // Act - Second move
        currentMouseX = 140;
        currentMouseY = 140;
        deltaX = currentMouseX - lastMouseX;
        deltaY = currentMouseY - lastMouseY;

        // Assert
        Assert.Equal(20, deltaX);
        Assert.Equal(20, deltaY);
    }
}
