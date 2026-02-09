using Atlas.Core.Abstractions;
using Atlas.Core.Models;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Providers;

public class CopilotProvider : ILlmProvider
{
    private readonly ILogger<CopilotProvider> _logger;

    public string Name => "Copilot";

    public CopilotProvider(ILogger<CopilotProvider> logger)
    {
        _logger = logger;
    }

    public async Task<string> SendMessageAsync(string message, ConversationContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await Cli.Wrap("gh")
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
        try
        {
            var result = await Cli.Wrap("which")
                .WithArguments("gh")
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync(cancellationToken);
            
            return result.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
