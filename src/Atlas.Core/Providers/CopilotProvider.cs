using Atlas.Core.Abstractions;
using Atlas.Core.Configuration;
using Atlas.Core.Models;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Providers;

public class CopilotProvider : ILlmProvider
{
    private readonly ILogger<CopilotProvider> _logger;
    private readonly ConfigurationService _configService;

    public string Name => "Copilot";

    public CopilotProvider(ILogger<CopilotProvider> logger, ConfigurationService configService)
    {
        _logger = logger;
        _configService = configService;
    }

    public async Task<string> SendMessageAsync(string message, ConversationContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await CliWrap.Cli.Wrap("gh")
                .WithArguments(new[] { "copilot", "suggest", message })
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync(cancellationToken);

            return result.StandardOutput;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling GitHub Copilot CLI");
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
        
        return await CommandHelper.IsCommandAvailableAsync("gh", cancellationToken);
    }
}
