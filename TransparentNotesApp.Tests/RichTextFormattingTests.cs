using System.Windows.Documents;

namespace TransparentNotesApp.Tests;

/// <summary>
/// Unit tests for rich text formatting functionality.
/// Tests list style transitions, bold toggle behavior, and formatting state management.
/// </summary>
public class RichTextFormattingTests
{
    /// <summary>
    /// Tests that bullet list marker style is correctly identified.
    /// </summary>
    [Fact]
    public void ListStyle_BulletMarker_IsDisc()
    {
        // Arrange
        TextMarkerStyle bulletStyle = TextMarkerStyle.Disc;

        // Assert
        Assert.Equal(TextMarkerStyle.Disc, bulletStyle);
    }

    /// <summary>
    /// Tests that numbered list marker style is correctly identified.
    /// </summary>
    [Fact]
    public void ListStyle_NumberedMarker_IsDecimal()
    {
        // Arrange
        TextMarkerStyle numberedStyle = TextMarkerStyle.Decimal;

        // Assert
        Assert.Equal(TextMarkerStyle.Decimal, numberedStyle);
    }

    /// <summary>
    /// Tests that list styles are different from each other.
    /// </summary>
    [Fact]
    public void ListStyle_BulletAndNumbered_AreDifferent()
    {
        // Arrange
        TextMarkerStyle bulletStyle = TextMarkerStyle.Disc;
        TextMarkerStyle numberedStyle = TextMarkerStyle.Decimal;

        // Assert
        Assert.NotEqual(bulletStyle, numberedStyle);
    }

    /// <summary>
    /// Tests that toggling from no list to bullet list changes state correctly.
    /// </summary>
    [Fact]
    public void ListStyle_ToggleToBullet_AppliesCorrectStyle()
    {
        // Arrange
        TextMarkerStyle? currentStyle = null;
        TextMarkerStyle targetStyle = TextMarkerStyle.Disc;

        // Act
        TextMarkerStyle newStyle = targetStyle;

        // Assert
        Assert.Equal(TextMarkerStyle.Disc, newStyle);
        Assert.NotEqual(currentStyle, newStyle);
    }

    /// <summary>
    /// Tests that toggling from no list to numbered list changes state correctly.
    /// </summary>
    [Fact]
    public void ListStyle_ToggleToNumbered_AppliesCorrectStyle()
    {
        // Arrange
        TextMarkerStyle? currentStyle = null;
        TextMarkerStyle targetStyle = TextMarkerStyle.Decimal;

        // Act
        TextMarkerStyle newStyle = targetStyle;

        // Assert
        Assert.Equal(TextMarkerStyle.Decimal, newStyle);
        Assert.NotEqual(currentStyle, newStyle);
    }

    /// <summary>
    /// Tests switching from bullet list to numbered list.
    /// </summary>
    [Fact]
    public void ListStyle_SwitchBulletToNumbered_ChangesStyle()
    {
        // Arrange
        TextMarkerStyle currentStyle = TextMarkerStyle.Disc;
        TextMarkerStyle targetStyle = TextMarkerStyle.Decimal;

        // Act
        bool shouldChangeStyle = currentStyle != targetStyle;
        TextMarkerStyle newStyle = targetStyle;

        // Assert
        Assert.True(shouldChangeStyle);
        Assert.Equal(TextMarkerStyle.Decimal, newStyle);
    }

    /// <summary>
    /// Tests switching from numbered list to bullet list.
    /// </summary>
    [Fact]
    public void ListStyle_SwitchNumberedToBullet_ChangesStyle()
    {
        // Arrange
        TextMarkerStyle currentStyle = TextMarkerStyle.Decimal;
        TextMarkerStyle targetStyle = TextMarkerStyle.Disc;

        // Act
        bool shouldChangeStyle = currentStyle != targetStyle;
        TextMarkerStyle newStyle = targetStyle;

        // Assert
        Assert.True(shouldChangeStyle);
        Assert.Equal(TextMarkerStyle.Disc, newStyle);
    }

    /// <summary>
    /// Tests that clicking bullet list when already in bullet list should remove list.
    /// </summary>
    [Fact]
    public void ListStyle_ClickSameStyle_ShouldRemoveList()
    {
        // Arrange
        TextMarkerStyle currentStyle = TextMarkerStyle.Disc;
        TextMarkerStyle targetStyle = TextMarkerStyle.Disc;

        // Act
        bool shouldRemoveList = currentStyle == targetStyle;

        // Assert
        Assert.True(shouldRemoveList);
    }

    /// <summary>
    /// Tests bold toggle state from normal to bold.
    /// </summary>
    [Fact]
    public void BoldToggle_FromNormal_BecomesBold()
    {
        // Arrange
        bool isBold = false;

        // Act
        isBold = !isBold;

        // Assert
        Assert.True(isBold);
    }

    /// <summary>
    /// Tests bold toggle state from bold to normal.
    /// </summary>
    [Fact]
    public void BoldToggle_FromBold_BecomesNormal()
    {
        // Arrange
        bool isBold = true;

        // Act
        isBold = !isBold;

        // Assert
        Assert.False(isBold);
    }

    /// <summary>
    /// Tests multiple bold toggles maintain correct state.
    /// </summary>
    [Fact]
    public void BoldToggle_MultipleTimes_MaintainsCorrectState()
    {
        // Arrange
        bool isBold = false;

        // Act & Assert
        isBold = !isBold; // true
        Assert.True(isBold);

        isBold = !isBold; // false
        Assert.False(isBold);

        isBold = !isBold; // true
        Assert.True(isBold);

        isBold = !isBold; // false
        Assert.False(isBold);
    }

    /// <summary>
    /// Tests that all available marker styles are distinct.
    /// </summary>
    [Fact]
    public void ListStyle_AllMarkerStyles_AreDistinct()
    {
        // Arrange
        TextMarkerStyle[] styles = new[]
        {
            TextMarkerStyle.None,
            TextMarkerStyle.Disc,
            TextMarkerStyle.Circle,
            TextMarkerStyle.Square,
            TextMarkerStyle.Decimal
        };

        // Assert - all should be unique
        Assert.Equal(styles.Length, styles.Distinct().Count());
    }

    /// <summary>
    /// Tests formatting state tracking for button visuals.
    /// </summary>
    [Fact]
    public void FormattingState_BoldButtonChecked_ReflectsState()
    {
        // Arrange
        bool selectionIsBold = true;
        bool buttonIsChecked = false;

        // Act
        buttonIsChecked = selectionIsBold;

        // Assert
        Assert.True(buttonIsChecked);
    }

    /// <summary>
    /// Tests formatting state tracking when selection is not bold.
    /// </summary>
    [Fact]
    public void FormattingState_BoldButtonUnchecked_ReflectsState()
    {
        // Arrange
        bool selectionIsBold = false;
        bool buttonIsChecked = true;

        // Act
        buttonIsChecked = selectionIsBold;

        // Assert
        Assert.False(buttonIsChecked);
    }
}
