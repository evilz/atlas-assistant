using Atlas.Core.Skills;
using Microsoft.Extensions.Logging.Abstractions;

namespace Atlas.Core.Tests;

public class FileOperationsSkillTests
{
    [Fact]
    public async Task ExecuteAsync_Read_ShouldReadFileContent()
    {
        // Arrange
        var skill = new FileOperationsSkill(NullLogger<FileOperationsSkill>.Instance);
        var testFile = Path.GetTempFileName();
        var testContent = "Test content";
        
        try
        {
            await File.WriteAllTextAsync(testFile, testContent);

            var parameters = new Dictionary<string, object>
            {
                ["operation"] = "read",
                ["path"] = testFile
            };

            // Act
            var result = await skill.ExecuteAsync(parameters);

            // Assert
            Assert.Contains(testContent, result);
        }
        finally
        {
            // Cleanup
            if (File.Exists(testFile))
                File.Delete(testFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_Write_ShouldWriteToFile()
    {
        // Arrange
        var skill = new FileOperationsSkill(NullLogger<FileOperationsSkill>.Instance);
        var testFile = Path.GetTempFileName();
        var testContent = "Written content";

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["operation"] = "write",
                ["path"] = testFile,
                ["content"] = testContent
            };

            // Act
            var result = await skill.ExecuteAsync(parameters);

            // Assert
            Assert.Contains("successfully", result);
            var fileContent = await File.ReadAllTextAsync(testFile);
            Assert.Equal(testContent, fileContent);
        }
        finally
        {
            // Cleanup
            if (File.Exists(testFile))
                File.Delete(testFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_List_ShouldListFiles()
    {
        // Arrange
        var skill = new FileOperationsSkill(NullLogger<FileOperationsSkill>.Instance);
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        
        try
        {
            Directory.CreateDirectory(testDir);
            var testFile = Path.Combine(testDir, "test.txt");
            await File.WriteAllTextAsync(testFile, "content");

            var parameters = new Dictionary<string, object>
            {
                ["operation"] = "list",
                ["path"] = testDir
            };

            // Act
            var result = await skill.ExecuteAsync(parameters);

            // Assert
            Assert.Contains("test.txt", result);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public async Task ExecuteAsync_MissingOperation_ShouldReturnError()
    {
        // Arrange
        var skill = new FileOperationsSkill(NullLogger<FileOperationsSkill>.Instance);
        var parameters = new Dictionary<string, object>();

        // Act
        var result = await skill.ExecuteAsync(parameters);

        // Assert
        Assert.Contains("Error", result);
        Assert.Contains("operation parameter is required", result);
    }
}
