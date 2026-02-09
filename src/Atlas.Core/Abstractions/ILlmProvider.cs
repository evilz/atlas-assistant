using Atlas.Core.Models;

namespace Atlas.Core.Abstractions;

public interface ILlmProvider
{
    string Name { get; }
    Task<string> SendMessageAsync(string message, ConversationContext? context = null, CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}
