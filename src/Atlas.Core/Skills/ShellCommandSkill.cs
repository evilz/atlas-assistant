using Atlas.Core.Abstractions;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Skills;

public class ShellCommandSkill : ISkill
{
    private readonly ILogger<ShellCommandSkill> _logger;
    
    // Whitelist of allowed commands for security
    private static readonly HashSet<string> AllowedCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        "ls", "dir", "pwd", "echo", "cat", "grep", "find", "head", "tail", "wc", "date"
    };

    public string Name => "ShellCommand";
    public string Description => "Execute whitelisted shell commands (ls, dir, pwd, echo, cat, grep, find, head, tail, wc, date)";

    public ShellCommandSkill(ILogger<ShellCommandSkill> logger)
    {
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
    {
        if (!parameters.TryGetValue("command", out var commandObj))
            return "Error: command parameter is required";

        var commandLine = commandObj.ToString()!;
        
        // Extract the command (first word)
        var commandParts = commandLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (commandParts.Length == 0)
            return "Error: command cannot be empty";

        var command = commandParts[0];
        
        // Security: Check if command is in whitelist
        if (!AllowedCommands.Contains(command))
        {
            _logger.LogWarning("Attempted to execute non-whitelisted command: {Command}", command);
            return $"Error: Command '{command}' is not allowed. Allowed commands: {string.Join(", ", AllowedCommands)}";
        }

        try
        {
            var result = await Cli.Wrap("/bin/bash")
                .WithArguments(new[] { "-c", commandLine })
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync(cancellationToken);

            return $"Exit Code: {result.ExitCode}\n\nOutput:\n{result.StandardOutput}\n\nError:\n{result.StandardError}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing shell command: {Command}", command);
            return $"Error: {ex.Message}";
        }
    }
}
