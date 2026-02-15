using Atlas.Core.Abstractions;
using Atlas.Core.Cron;
using Atlas.Core.Memory;
using Atlas.Core.Messaging;
using Atlas.Core.Providers;
using Atlas.Core.Skills;
using Atlas.Core.Installers;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAtlasCore(this IServiceCollection services)
    {
        // Register Configuration Service
        services.AddSingleton<Configuration.ConfigurationService>();
        // Register LLM Providers
        services.AddSingleton<ILlmProvider, CodexProvider>();
        services.AddSingleton<ILlmProvider, ClaudeProvider>();
        services.AddSingleton<ILlmProvider, GeminiProvider>();
        services.AddSingleton<ILlmProvider, CopilotProvider>();

        // Register Provider Installers
        services.AddSingleton<IProviderInstaller, GeminiInstaller>();
        services.AddSingleton<IProviderInstaller, ClaudeInstaller>();
        services.AddSingleton<IProviderInstaller, CopilotInstaller>();
        services.AddSingleton<IProviderInstaller, CodexInstaller>();

        // Register Installer Factory
        services.AddSingleton<ProviderInstallerFactory>();
        
        // Register CLI Helpers
        services.AddTransient<Cli.SetupWizard>();
        services.AddTransient<Cli.CliService>();
        services.AddTransient<Cli.TerminalGuiService>();


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

        // Register Cron Scheduler with async factory
        services.AddSingleton(sp =>
        {
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<CronScheduler>>();
            return CronScheduler.CreateAsync(logger).GetAwaiter().GetResult();
        });

        return services;
    }
}
