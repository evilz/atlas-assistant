
using Atlas.Core.Abstractions;
using Atlas.Core.Helpers;
using Atlas.Core.Providers;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Installers;

public class GeminiInstaller : IProviderInstaller
{
    private readonly ILogger<GeminiInstaller> _logger;

    public string ProviderName => "Gemini";

    public GeminiInstaller(ILogger<GeminiInstaller> logger)
    {
        _logger = logger;
    }

    public async Task<bool> IsInstalledAsync()
    {
        return await CommandHelper.IsCommandAvailableAsync("gemini");
    }

    public async Task<bool> InstallAsync()
    {
        if (!await NodeJsDetector.IsNodeInstalledAsync())
        {
            _logger.LogError("Node.js and npm are required to install Gemini CLI.");
            return false;
        }

        try
        {
            _logger.LogInformation("Installing Gemini CLI via npm...");
            var result = await Cli.Wrap("npm")
                .WithArguments("install -g @google/gemini-cli")
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync();

            if (result.ExitCode != 0)
            {
                _logger.LogError("Failed to install Gemini CLI: {Error}", result.StandardError);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Gemini CLI installation");
            return false;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        // Check if Gemini CLI is responsive and configured.
        // For @google/gemini-cli, it often relies on checking if it can start.
        // Since we don't have a dedicated 'auth status' command for this specific CLI wrapper without side effects,
        // we'll check if the process can start and ideally if GEMINI_API_KEY is present or if a config file exists.
        // For now, we'll assume if it's installed and we can run --version, it's "ready" enough for the wizard context, 
        // OR we can check for environment variables.
        
        bool installed = await IsInstalledAsync();
        if (!installed) return false;

        // Simple check: Environment variable
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GEMINI_API_KEY")))
        {
            return true;
        }

        return false;
    }

    public async Task AuthenticateAsync()
    {
        try
        {
            _logger.LogInformation("Launching Gemini CLI...");
            
            // Launch the gemini command interactively
            var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "gemini",
                UseShellExecute = true
            });
           
            if (process != null)
            {
                await process.WaitForExitAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error launching Gemini CLI.");
        }
    }
}
