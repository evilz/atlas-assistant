
using Atlas.Core.Abstractions;
using Atlas.Core.Helpers;
using Atlas.Core.Providers;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Installers;

public class CopilotInstaller : IProviderInstaller
{
    private readonly ILogger<CopilotInstaller> _logger;

    public string ProviderName => "Copilot";

    public CopilotInstaller(ILogger<CopilotInstaller> logger)
    {
        _logger = logger;
    }

    public async Task<bool> IsInstalledAsync()
    {
        return await CommandHelper.IsCommandAvailableAsync("gh");
    }

    public async Task<bool> InstallAsync()
    {
        if (!await NodeJsDetector.IsNodeInstalledAsync())
        {
            _logger.LogError("Node.js and npm are required to install GitHub Copilot CLI extension.");
            return false;
        }

         try
        {
            _logger.LogInformation("Installing GitHub Copilot CLI extension via npm...");
             var result = await CliWrap.Cli.Wrap("npm")
                .WithArguments("install -g @github/copilot")
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync();

            if (result.ExitCode != 0)
            {
                 _logger.LogError("Failed to install GitHub Copilot CLI: {Error}", result.StandardError);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during GitHub Copilot CLI installation");
            return false;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        if (!await IsInstalledAsync()) return false;

        try
        {
            var result = await CliWrap.Cli.Wrap("gh")
                .WithArguments("auth status")
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync();

            return result.ExitCode == 0;
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
            _logger.LogInformation("Launching GitHub authentication...");
            // Use 'gh auth login' which is interactive
            // Since we are in a CLI app, we should ideally let the process inherit standard input/output
            // But CliWrap default ExecuteAsync might not be fully interactive bridging.
            // However, 'gh' often requires web browser flow.
            
           var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
           {
               FileName = "gh",
               Arguments = "auth login",
               UseShellExecute = false
           });
           
           if (process != null)
           {
               await process.WaitForExitAsync();
           }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error launching authentication for Copilot.");
        }
    }
}
