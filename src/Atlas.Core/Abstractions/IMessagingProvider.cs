namespace Atlas.Core.Abstractions;

public interface IMessagingProvider
{
    string Name { get; }
    Task SendMessageAsync(string recipient, string message, CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}
