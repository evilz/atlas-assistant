using Atlas.Core.Abstractions;
using Atlas.Core.Models;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Providers;

public class GeminiProvider : ILlmProvider
{
    private readonly ILogger<GeminiProvider> _logger;

    public string Name => "Gemini";

    public GeminiProvider(ILogger<GeminiProvider> logger)
    {
        _logger = logger;
    }

    public async Task<string> SendMessageAsync(string message, ConversationContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await Cli.Wrap("gemini")
                .WithArguments(message)
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync(cancellationToken);

            return result.StandardOutput;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Gemini CLI");
            throw;
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        return await CommandHelper.IsCommandAvailableAsync("gemini", cancellationToken);
    }
}
