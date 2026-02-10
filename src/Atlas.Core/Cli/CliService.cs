
using Microsoft.Extensions.Logging;
using Atlas.Core.Abstractions;

namespace Atlas.Core.Cli;

public class CliService
{
    private readonly AtlasOrchestrator _orchestrator;
    private readonly ILogger<CliService> _logger;

    public CliService(AtlasOrchestrator orchestrator, ILogger<CliService> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    public async Task RunInteractiveModeAsync()
    {
        _logger.LogInformation("ATLAS Assistant - Ultra-lightweight AI Control System");
        _logger.LogInformation("=================================================");

        // Display available providers
        _logger.LogInformation("\nAvailable LLM Providers:");
        foreach (var provider in _orchestrator.GetAvailableProviders())
        {
            _logger.LogInformation("  - {Provider}", provider);
        }

        // Display available skills
        _logger.LogInformation("\nAvailable Skills:");
        foreach (var skill in _orchestrator.GetAvailableSkills())
        {
            _logger.LogInformation("  - {Skill}", skill);
        }

        _logger.LogInformation("\nEnter your message (or 'quit' to exit):");
        _logger.LogInformation("Tip: Run with --setup to configure AI providers.\n");
        
        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input) || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                break;

            try
            {
                var response = await _orchestrator.ProcessMessageAsync(input);
                Console.WriteLine($"\nResponse: {response}\n");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        }
    }

    public async Task RunSingleCommandAsync(string message)
    {
         try
        {
            var response = await _orchestrator.ProcessMessageAsync(message);
            Console.WriteLine(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
            throw; 
        }
    }
}
