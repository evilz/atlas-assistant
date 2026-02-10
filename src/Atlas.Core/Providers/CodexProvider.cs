using System.Text;
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
                .WithArguments(["exec","--json", message])
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync(cancellationToken);

            // Access stdout & stderr buffered in-memory as strings
            if (result.StandardError.Contains("status 401 Unauthorized"))
            {
                var r2 = await Cli.Wrap("codex")
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
        return await CommandHelper.IsCommandAvailableAsync("codex", cancellationToken);
    }
}
