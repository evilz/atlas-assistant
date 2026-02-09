using Atlas.Core.Abstractions;
using Atlas.Core.Cron;
using Atlas.Core.Memory;
using Atlas.Core.Messaging;
using Atlas.Core.Providers;
using Atlas.Core.Skills;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAtlasCore(this IServiceCollection services)
    {
        // Register LLM Providers
        services.AddSingleton<ILlmProvider, ClaudeProvider>();
        services.AddSingleton<ILlmProvider, GeminiProvider>();
        services.AddSingleton<ILlmProvider, CopilotProvider>();
        services.AddSingleton<ILlmProvider, CodexProvider>();

        // Register Messaging Providers
        services.AddSingleton<IMessagingProvider, TelegramProvider>();
        services.AddSingleton<IMessagingProvider, WhatsAppProvider>();

        // Register Skills
        services.AddSingleton<ISkill, FileOperationsSkill>();
        services.AddSingleton<ISkill, ShellCommandSkill>();

        // Register Memory Store
        services.AddSingleton<IMemoryStore, FileMemoryStore>();

        // Register Orchestrator
        services.AddSingleton<AtlasOrchestrator>();

        // Register Cron Scheduler
        services.AddSingleton<CronScheduler>();

        return services;
    }
}
