using Atlas.Core.Abstractions;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Skills;

public class ShellCommandSkill : ISkill
{
    private readonly ILogger<ShellCommandSkill> _logger;

    public string Name => "ShellCommand";
    public string Description => "Execute shell commands";

    public ShellCommandSkill(ILogger<ShellCommandSkill> logger)
    {
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
    {
        if (!parameters.TryGetValue("command", out var command))
            return "Error: command parameter is required";

        try
        {
            var result = await Cli.Wrap("/bin/bash")
                .WithArguments(new[] { "-c", command.ToString()! })
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
