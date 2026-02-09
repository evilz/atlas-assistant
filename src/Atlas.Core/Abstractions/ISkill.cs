namespace Atlas.Core.Abstractions;

public interface ISkill
{
    string Name { get; }
    string Description { get; }
    Task<string> ExecuteAsync(Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
}
