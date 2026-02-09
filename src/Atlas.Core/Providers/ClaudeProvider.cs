using Atlas.Core.Abstractions;
using Atlas.Core.Models;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Providers;

public class ClaudeProvider : ILlmProvider
{
    private readonly ILogger<ClaudeProvider> _logger;

    public string Name => "Claude";

    public ClaudeProvider(ILogger<ClaudeProvider> logger)
    {
        _logger = logger;
    }

    public async Task<string> SendMessageAsync(string message, ConversationContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await Cli.Wrap("claude")
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
        return await CommandHelper.IsCommandAvailableAsync("claude", cancellationToken);
    }
}
