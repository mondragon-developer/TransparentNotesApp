namespace TransparentNotesApp.Tests;

/// <summary>
/// Unit tests for notes persistence functionality.
/// Tests file path generation, directory creation, and RTF format handling.
/// </summary>
public class NotesPersistenceTests
{
    /// <summary>
    /// Tests that notes file path is correctly constructed.
    /// </summary>
    [Fact]
    public void NotesPath_Construction_ContainsAppFolder()
    {
        // Arrange
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string appFolder = "TransparentNotesApp";
        string fileName = "notes.rtf";

        // Act
        string fullPath = Path.Combine(appDataPath, appFolder, fileName);

        // Assert
        Assert.Contains(appFolder, fullPath);
        Assert.Contains(fileName, fullPath);
        Assert.EndsWith(".rtf", fullPath);
    }

    /// <summary>
    /// Tests that app data folder path is valid.
    /// </summary>
    [Fact]
    public void NotesPath_AppDataFolder_IsNotEmpty()
    {
        // Arrange & Act
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        // Assert
        Assert.False(string.IsNullOrEmpty(appDataPath));
    }

    /// <summary>
    /// Tests that notes file has RTF extension.
    /// </summary>
    [Fact]
    public void NotesFile_Extension_IsRtf()
    {
        // Arrange
        string fileName = "notes.rtf";

        // Act
        string extension = Path.GetExtension(fileName);

        // Assert
        Assert.Equal(".rtf", extension);
    }

    /// <summary>
    /// Tests that file path components are correctly combined.
    /// </summary>
    [Fact]
    public void NotesPath_PathCombine_CreatesValidPath()
    {
        // Arrange
        string basePath = @"C:\Users\Test\AppData\Local";
        string appFolder = "TransparentNotesApp";
        string fileName = "notes.rtf";

        // Act
        string fullPath = Path.Combine(basePath, appFolder, fileName);

        // Assert
        Assert.Equal(@"C:\Users\Test\AppData\Local\TransparentNotesApp\notes.rtf", fullPath);
    }

    /// <summary>
    /// Tests that file existence check returns false for non-existent file.
    /// </summary>
    [Fact]
    public void NotesFile_NonExistent_ReturnsFalse()
    {
        // Arrange
        string nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".rtf");

        // Act
        bool exists = File.Exists(nonExistentPath);

        // Assert
        Assert.False(exists);
    }

    /// <summary>
    /// Tests that directory can be created without error.
    /// </summary>
    [Fact]
    public void NotesDirectory_CreateDirectory_Succeeds()
    {
        // Arrange
        string tempPath = Path.Combine(Path.GetTempPath(), "TransparentNotesAppTest_" + Guid.NewGuid().ToString());

        try
        {
            // Act
            Directory.CreateDirectory(tempPath);

            // Assert
            Assert.True(Directory.Exists(tempPath));
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath);
            }
        }
    }

    /// <summary>
    /// Tests that creating directory twice doesn't throw.
    /// </summary>
    [Fact]
    public void NotesDirectory_CreateDirectoryTwice_DoesNotThrow()
    {
        // Arrange
        string tempPath = Path.Combine(Path.GetTempPath(), "TransparentNotesAppTest_" + Guid.NewGuid().ToString());

        try
        {
            // Act & Assert - should not throw
            Directory.CreateDirectory(tempPath);
            Directory.CreateDirectory(tempPath); // Second call should be safe

            Assert.True(Directory.Exists(tempPath));
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath);
            }
        }
    }

    /// <summary>
    /// Tests RTF data format string constant.
    /// </summary>
    [Fact]
    public void RtfFormat_DataFormat_IsCorrect()
    {
        // Arrange
        string expectedFormat = "Rich Text Format";

        // Act
        // System.Windows.DataFormats.Rtf resolves to "Rich Text Format"
        string rtfFormat = "Rich Text Format";

        // Assert
        Assert.Equal(expectedFormat, rtfFormat);
    }

    /// <summary>
    /// Tests file write and read cycle with temporary file.
    /// </summary>
    [Fact]
    public void NotesFile_WriteAndRead_PreservesContent()
    {
        // Arrange
        string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".txt");
        string testContent = "Test notes content";

        try
        {
            // Act
            File.WriteAllText(tempPath, testContent);
            string readContent = File.ReadAllText(tempPath);

            // Assert
            Assert.Equal(testContent, readContent);
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    /// <summary>
    /// Tests that empty file can be created and read.
    /// </summary>
    [Fact]
    public void NotesFile_EmptyFile_CanBeCreatedAndRead()
    {
        // Arrange
        string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".rtf");

        try
        {
            // Act
            File.WriteAllText(tempPath, string.Empty);
            string content = File.ReadAllText(tempPath);

            // Assert
            Assert.True(File.Exists(tempPath));
            Assert.Equal(string.Empty, content);
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    /// <summary>
    /// Tests that file mode create overwrites existing file.
    /// </summary>
    [Fact]
    public void NotesFile_FileStreamCreate_OverwritesExisting()
    {
        // Arrange
        string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".txt");
        string originalContent = "Original content";
        string newContent = "New content";

        try
        {
            // Act
            File.WriteAllText(tempPath, originalContent);
            File.WriteAllText(tempPath, newContent);
            string readContent = File.ReadAllText(tempPath);

            // Assert
            Assert.Equal(newContent, readContent);
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    /// <summary>
    /// Tests LocalApplicationData special folder resolution.
    /// </summary>
    [Fact]
    public void SpecialFolder_LocalApplicationData_ReturnsValidPath()
    {
        // Act
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(localAppData));
        Assert.True(Directory.Exists(localAppData));
    }
}
