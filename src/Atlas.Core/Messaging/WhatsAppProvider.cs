using Atlas.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Messaging;

public class WhatsAppProvider : IMessagingProvider
{
    private readonly ILogger<WhatsAppProvider> _logger;
    private readonly string? _apiKey;

    public string Name => "WhatsApp";

    public WhatsAppProvider(ILogger<WhatsAppProvider> logger, string? apiKey = null)
    {
        _logger = logger;
        _apiKey = apiKey ?? Environment.GetEnvironmentVariable("WHATSAPP_API_KEY");
    }

    public async Task SendMessageAsync(string recipient, string message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogWarning("WhatsApp API key not configured");
            return;
        }

        try
        {
            // Placeholder for WhatsApp Business API integration
            _logger.LogInformation("Sending WhatsApp message to {Recipient}: {Message}", recipient, message);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending WhatsApp message");
            throw;
        }
    }

    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(!string.IsNullOrEmpty(_apiKey));
    }
}
