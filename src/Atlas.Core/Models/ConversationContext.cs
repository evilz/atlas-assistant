namespace Atlas.Core.Models;

public class ConversationContext
{
    public string ConversationId { get; set; } = Guid.NewGuid().ToString();
    public List<Message> Messages { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}
