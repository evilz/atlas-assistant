using Atlas.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Messaging;

public class TelegramProvider : IMessagingProvider
{
    private readonly ILogger<TelegramProvider> _logger;
    private readonly string? _botToken;

    public string Name => "Telegram";

    public TelegramProvider(ILogger<TelegramProvider> logger, string? botToken = null)
    {
        _logger = logger;
        _botToken = botToken ?? Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
    }

    public async Task SendMessageAsync(string recipient, string message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_botToken))
        {
            _logger.LogWarning("Telegram bot token not configured");
            return;
        }

        try
        {
            // Placeholder for Telegram API integration
            // In a real implementation, this would use Telegram.Bot package
            _logger.LogInformation("Sending Telegram message to {Recipient}: {Message}", recipient, message);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending Telegram message");
            throw;
        }
    }

    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(!string.IsNullOrEmpty(_botToken));
    }
}
