using Atlas.Core.Abstractions;
using Atlas.Core.Models;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Providers;

public class CodexProvider : ILlmProvider
{
    private readonly ILogger<CodexProvider> _logger;

    public string Name => "Codex";

    public CodexProvider(ILogger<CodexProvider> logger)
    {
        _logger = logger;
    }

    public async Task<string> SendMessageAsync(string message, ConversationContext? context = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await Cli.Wrap("codex")
                .WithArguments(message)
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync(cancellationToken);

            return result.StandardOutput;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Codex CLI");
            throw;
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await Cli.Wrap("which")
                .WithArguments("codex")
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
