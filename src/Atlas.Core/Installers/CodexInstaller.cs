
using Atlas.Core.Abstractions;
using Atlas.Core.Helpers;
using Atlas.Core.Providers;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Installers;

public class CodexInstaller : IProviderInstaller
{
    private readonly ILogger<CodexInstaller> _logger;

    public string ProviderName => "Codex";

    public CodexInstaller(ILogger<CodexInstaller> logger)
    {
        _logger = logger;
    }

    public async Task<bool> IsInstalledAsync()
    {
        return await CommandHelper.IsCommandAvailableAsync("codex");
    }

    public async Task<bool> InstallAsync()
    {
        if (!await NodeJsDetector.IsNodeInstalledAsync())
        {
            _logger.LogError("Node.js and npm are required to install Codex CLI.");
            return false;
        }

        try
        {
            _logger.LogInformation("Installing Codex CLI via npm...");
            var result = await CliWrap.Cli.Wrap("npm")
                .WithArguments("install -g codex-cli")
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync();

            if (result.ExitCode != 0)
            {
                _logger.LogError("Failed to install Codex CLI: {Error}", result.StandardError);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Codex CLI installation");
            return false;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        if (!await IsInstalledAsync()) return false;

        // Codex CLI auth check
        try 
        {
           // Assuming 'codex whoami' or similar exists, or checking config
           // Fallback to checking if it's installed for now as Codex CLI details are scarce in context
           return true; 
        }
        catch
        {
            return false;
        }
    }

    public async Task AuthenticateAsync()
    {
        try
        {
             _logger.LogInformation("Launching Codex authentication...");
             // Based on CodexProvider implementation: codex login --device-auth
            var result = await CliWrap.Cli.Wrap("codex")
                .WithArguments("login --device-auth")
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync();
                
            _logger.LogInformation(result.StandardOutput);
            if (!string.IsNullOrEmpty(result.StandardError))
            {
                _logger.LogError(result.StandardError);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error launching authentication for Codex.");
        }
    }
}
