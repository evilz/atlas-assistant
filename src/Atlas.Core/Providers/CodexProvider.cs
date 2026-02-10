using System.Text;
using Atlas.Core.Abstractions;
using Atlas.Core.Configuration;
using Atlas.Core.Models;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Providers;

public class CodexProvider : ILlmProvider
{
    private readonly ILogger<CodexProvider> _logger;
    private readonly ConfigurationService _configService;

    public string Name => "Codex";

    public CodexProvider(ILogger<CodexProvider> logger, ConfigurationService configService)
    {
        _logger = logger;
        _configService = configService;
    }

    public async Task<string> SendMessageAsync(string message, ConversationContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {

            
            var result = await CliWrap.Cli.Wrap("codex")
                .WithArguments(["exec","--json", message])
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync(cancellationToken);

            // Access stdout & stderr buffered in-memory as strings
            if (result.StandardError.Contains("status 401 Unauthorized"))
            {
                var r2 = await CliWrap.Cli.Wrap("codex")
                    .WithArguments(["login","--device-auth"])
                   .ExecuteBufferedAsync(cancellationToken);
            }
            return result.StandardOutput;;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Codex CLI");
            throw;
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        // Check if enabled in config
        if (_configService.CurrentConfig.Providers.TryGetValue(Name, out var config) && !config.Enabled)
        {
            return false;
        }
        
        return await CommandHelper.IsCommandAvailableAsync("codex", cancellationToken);
    }
}
