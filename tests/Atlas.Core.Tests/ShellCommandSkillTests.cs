using Atlas.Core.Skills;
using Microsoft.Extensions.Logging.Abstractions;

namespace Atlas.Core.Tests;

public class ShellCommandSkillTests
{
    [Fact]
    public async Task ExecuteAsync_AllowedCommand_ShouldExecute()
    {
        // Arrange
        var skill = new ShellCommandSkill(NullLogger<ShellCommandSkill>.Instance);
        var parameters = new Dictionary<string, object>
        {
            ["command"] = "echo test"
        };

        // Act
        var result = await skill.ExecuteAsync(parameters);

        // Assert
        Assert.Contains("Exit Code:", result);
        Assert.DoesNotContain("Error: ", result); // Note the space after Error:
    }

    [Fact]
    public async Task ExecuteAsync_DisallowedCommand_ShouldReturnError()
    {
        // Arrange
        var skill = new ShellCommandSkill(NullLogger<ShellCommandSkill>.Instance);
        var parameters = new Dictionary<string, object>
        {
            ["command"] = "rm -rf /"
        };

        // Act
        var result = await skill.ExecuteAsync(parameters);

        // Assert
        Assert.Contains("Error:", result);
        Assert.Contains("not allowed", result);
    }

    [Fact]
    public async Task ExecuteAsync_CommandChaining_ShouldReturnError()
    {
        // Arrange
        var skill = new ShellCommandSkill(NullLogger<ShellCommandSkill>.Instance);
        var parameters = new Dictionary<string, object>
        {
            ["command"] = "ls && echo test"
        };

        // Act
        var result = await skill.ExecuteAsync(parameters);

        // Assert
        Assert.Contains("Error:", result);
        Assert.Contains("Shell metacharacters", result);
    }

    [Fact]
    public async Task ExecuteAsync_PipeOperator_ShouldReturnError()
    {
        // Arrange
        var skill = new ShellCommandSkill(NullLogger<ShellCommandSkill>.Instance);
        var parameters = new Dictionary<string, object>
        {
            ["command"] = "ls | grep test"
        };

        // Act
        var result = await skill.ExecuteAsync(parameters);

        // Assert
        Assert.Contains("Error:", result);
        Assert.Contains("Shell metacharacters", result);
    }

    [Fact]
    public async Task ExecuteAsync_Semicolon_ShouldReturnError()
    {
        // Arrange
        var skill = new ShellCommandSkill(NullLogger<ShellCommandSkill>.Instance);
        var parameters = new Dictionary<string, object>
        {
            ["command"] = "ls ; echo test"  // Space ensures semicolon is detected
        };

        // Act
        var result = await skill.ExecuteAsync(parameters);

        // Assert
        Assert.Contains("Error:", result);
        Assert.Contains("Shell metacharacters", result);
    }

    [Fact]
    public async Task ExecuteAsync_MissingCommand_ShouldReturnError()
    {
        // Arrange
        var skill = new ShellCommandSkill(NullLogger<ShellCommandSkill>.Instance);
        var parameters = new Dictionary<string, object>();

        // Act
        var result = await skill.ExecuteAsync(parameters);

        // Assert
        Assert.Contains("Error:", result);
        Assert.Contains("command parameter is required", result);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyCommand_ShouldReturnError()
    {
        // Arrange
        var skill = new ShellCommandSkill(NullLogger<ShellCommandSkill>.Instance);
        var parameters = new Dictionary<string, object>
        {
            ["command"] = "   "
        };

        // Act
        var result = await skill.ExecuteAsync(parameters);

        // Assert
        Assert.Contains("Error:", result);
        Assert.Contains("cannot be empty", result);
    }
}
