using Atlas.Core.Abstractions;
using Atlas.Core.Models;
using Microsoft.Extensions.Logging;

namespace Atlas.Core;

public class AtlasOrchestrator
{
    private readonly IEnumerable<ILlmProvider> _llmProviders;
    private readonly IEnumerable<IMessagingProvider> _messagingProviders;
    private readonly IEnumerable<ISkill> _skills;
    private readonly IMemoryStore _memoryStore;
    private readonly ILogger<AtlasOrchestrator> _logger;

    public AtlasOrchestrator(
        IEnumerable<ILlmProvider> llmProviders,
        IEnumerable<IMessagingProvider> messagingProviders,
        IEnumerable<ISkill> skills,
        IMemoryStore memoryStore,
        ILogger<AtlasOrchestrator> logger)
    {
        _llmProviders = llmProviders;
        _messagingProviders = messagingProviders;
        _skills = skills;
        _memoryStore = memoryStore;
        _logger = logger;
    }

    public async Task<string> ProcessMessageAsync(string message, string? preferredProvider = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing message with preferred provider: {Provider}", preferredProvider ?? "auto");

        ILlmProvider? provider = null;

        if (!string.IsNullOrEmpty(preferredProvider))
        {
            provider = _llmProviders.FirstOrDefault(p => p.Name.Equals(preferredProvider, StringComparison.OrdinalIgnoreCase));
        }

        if (provider == null)
        {
            // Find first available provider
            foreach (var p in _llmProviders)
            {
                if (await p.IsAvailableAsync(cancellationToken))
                {
                    provider = p;
                    break;
                }
            }
        }

        if (provider == null)
        {
            throw new InvalidOperationException("No LLM provider is available");
        }

        _logger.LogInformation("Using provider: {Provider}", provider.Name);
        var response = await provider.SendMessageAsync(message, null, cancellationToken);
        
        return response;
    }

    public async Task<string> ExecuteSkillAsync(string skillName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
    {
        var skill = _skills.FirstOrDefault(s => s.Name.Equals(skillName, StringComparison.OrdinalIgnoreCase));
        
        if (skill == null)
        {
            throw new InvalidOperationException($"Skill not found: {skillName}");
        }

        _logger.LogInformation("Executing skill: {Skill}", skillName);
        return await skill.ExecuteAsync(parameters, cancellationToken);
    }

    public async Task SendNotificationAsync(string message, string? preferredChannel = null, CancellationToken cancellationToken = default)
    {
        IMessagingProvider? provider = null;

        if (!string.IsNullOrEmpty(preferredChannel))
        {
            provider = _messagingProviders.FirstOrDefault(p => p.Name.Equals(preferredChannel, StringComparison.OrdinalIgnoreCase));
        }

        if (provider == null)
        {
            provider = _messagingProviders.FirstOrDefault(p => p.IsAvailableAsync(cancellationToken).Result);
        }

        if (provider != null)
        {
            await provider.SendMessageAsync("default", message, cancellationToken);
        }
        else
        {
            _logger.LogWarning("No messaging provider available");
        }
    }

    public IEnumerable<string> GetAvailableProviders()
    {
        return _llmProviders.Select(p => p.Name);
    }

    public IEnumerable<string> GetAvailableSkills()
    {
        return _skills.Select(s => $"{s.Name}: {s.Description}");
    }
}
