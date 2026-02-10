using Atlas.Core.Abstractions;
using Atlas.Core.Configuration;
using Atlas.Core.Models;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Providers;

public class ClaudeProvider : ILlmProvider
{
    private readonly ILogger<ClaudeProvider> _logger;
    private readonly ConfigurationService _configService;

    public string Name => "Claude";

    public ClaudeProvider(ILogger<ClaudeProvider> logger, ConfigurationService configService)
    {
        _logger = logger;
        _configService = configService;
    }

    public async Task<string> SendMessageAsync(string message, ConversationContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await CliWrap.Cli.Wrap("claude")
                .WithArguments(message)
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync(cancellationToken);

            return result.StandardOutput;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Claude CLI");
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
        
        return await CommandHelper.IsCommandAvailableAsync("claude", cancellationToken);
    }
}
