
using Atlas.Core.Abstractions;
using Atlas.Core.Helpers;
using Atlas.Core.Providers;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Installers;

public class ClaudeInstaller : IProviderInstaller
{
    private readonly ILogger<ClaudeInstaller> _logger;

    public string ProviderName => "Claude";

    public ClaudeInstaller(ILogger<ClaudeInstaller> logger)
    {
        _logger = logger;
    }

    public async Task<bool> IsInstalledAsync()
    {
        return await CommandHelper.IsCommandAvailableAsync("claude");
    }

    public async Task<bool> InstallAsync()
    {
        if (!await NodeJsDetector.IsNodeInstalledAsync())
        {
            _logger.LogError("Node.js and npm are required to install Claude CLI.");
            return false;
        }

        try
        {
            _logger.LogInformation("Installing Claude CLI via npm...");
            var result = await Cli.Wrap("npm")
                .WithArguments("install -g @anthropic-ai/claude-cli")
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync();

            if (result.ExitCode != 0)
            {
                _logger.LogError("Failed to install Claude CLI: {Error}", result.StandardError);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Claude CLI installation");
            return false;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        if (!await IsInstalledAsync()) return false;

        // Check for API Key in environment
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")))
        {
            return true;
        }
        
        // Or check `claude auth token` if available (mocking for now as we don't have the CLI doc perfectly)
        return false;
    }

    public async Task AuthenticateAsync()
    {
        try 
        {
            // Claude CLI often has a login command
            await Cli.Wrap("claude")
                .WithArguments("login")
                .WithStandardInputPipe(PipeSource.Null) // Ensure it doesn't hang waiting for input if not interactive in this context
                // But this is running in CLI, so we want it to be interactive? 
                // Creating a process that shares the console might be better for interactive login.
                // For now, let's just try to run it.
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync(); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error launching authentication for Claude.");
        }
    }
}
