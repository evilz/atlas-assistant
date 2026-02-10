using Atlas.CLI;
using Atlas.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Add Atlas Core services
// Add Atlas Core services
builder.Services.AddAtlasCore();

// Register CLI services
builder.Services.AddSingleton<ProviderWizard>();

// Add logging
builder.Logging.AddConsole();

var host = builder.Build();

var orchestrator = host.Services.GetRequiredService<AtlasOrchestrator>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("ATLAS Assistant - Ultra-lightweight AI Control System");
logger.LogInformation("=================================================");

// Display available providers
logger.LogInformation("\nAvailable LLM Providers:");
foreach (var provider in orchestrator.GetAvailableProviders())
{
    logger.LogInformation("  - {Provider}", provider);
}

// Display available skills
logger.LogInformation("\nAvailable Skills:");
foreach (var skill in orchestrator.GetAvailableSkills())
{
    logger.LogInformation("  - {Skill}", skill);
}

// Check for setup flag
if (args.Contains("--setup"))
{
    var wizard = host.Services.GetRequiredService<ProviderWizard>();
    await wizard.RunAsync();
    return 0;
}

// Interactive mode if no args provided
if (args.Length == 0)
{
    logger.LogInformation("\nEnter your message (or 'quit' to exit):");
    logger.LogInformation("Tip: Run with --setup to configure AI providers.\n");
    
    while (true)
    {
        Console.Write("> ");
        var input = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(input) || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
            break;

        try
        {
            var response = await orchestrator.ProcessMessageAsync(input);
            Console.WriteLine($"\nResponse: {response}\n");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message");
        }
    }
}
else
{
    // Process single message from args
    var message = string.Join(" ", args);
    try
    {
        var response = await orchestrator.ProcessMessageAsync(message);
        Console.WriteLine(response);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error processing message");
        return 1;
    }
}

return 0;
