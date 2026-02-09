# ATLAS Usage Examples

This guide provides practical examples of using ATLAS Assistant.

## CLI Usage

### Interactive Mode

Start the CLI without arguments to enter interactive mode:

```bash
dotnet run --project src/Atlas.CLI
```

You'll see:
```
ATLAS Assistant - Ultra-lightweight AI Control System
=================================================

Available LLM Providers:
  - Claude
  - Gemini
  - Copilot
  - Codex

Available Skills:
  - FileOperations: Perform file system operations like read, write, list
  - ShellCommand: Execute shell commands

Enter your message (or 'quit' to exit):
>
```

Type your questions or commands:
```
> What is the capital of France?
> quit
```

### Single Command Mode

Execute a single command:

```bash
dotnet run --project src/Atlas.CLI "Explain how async/await works in C#"
```

## Web UI Usage

### Starting the Web Interface

```bash
dotnet run --project src/Atlas.Web
```

Navigate to `https://localhost:5001` in your browser.

### Available Pages

1. **Home** (`/`) - Landing page with overview
2. **Dashboard** (`/dashboard`) - System status and available resources
3. **Chat** (`/chat`) - Interactive chat interface

### Using the Chat Interface

1. Navigate to `/chat`
2. Select your preferred LLM provider from the left sidebar
3. Type your message in the input box
4. Press Enter or click Send
5. View the AI response in the conversation area

## Programmatic Usage

### Using in Your Own .NET Application

```csharp
using Atlas.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddAtlasCore();
var host = builder.Build();

var orchestrator = host.Services.GetRequiredService<AtlasOrchestrator>();

// Send a message to an LLM
var response = await orchestrator.ProcessMessageAsync("Hello, how are you?");
Console.WriteLine(response);

// Execute a skill
var fileList = await orchestrator.ExecuteSkillAsync("FileOperations", 
    new Dictionary<string, object> 
    {
        ["operation"] = "list",
        ["path"] = "/tmp"
    });
Console.WriteLine(fileList);
```

### Custom Agent Implementation

```csharp
using Atlas.Core.Abstractions;
using Atlas.Core.Models;

public class MyCustomAgent
{
    private readonly IEnumerable<ILlmProvider> _providers;
    private readonly IMemoryStore _memory;

    public MyCustomAgent(
        IEnumerable<ILlmProvider> providers,
        IMemoryStore memory)
    {
        _providers = providers;
        _memory = memory;
    }

    public async Task<string> PerformComplexTaskAsync(string input)
    {
        // Get preferred provider
        var provider = _providers.First();
        
        // Load context from memory
        var context = await _memory.GetAsync("conversation_context");
        
        // Process with LLM
        var response = await provider.SendMessageAsync(input);
        
        // Save to memory
        await _memory.SaveAsync("last_response", response);
        
        return response;
    }
}
```

## Skills Usage

### FileOperations Skill

Read a file:
```csharp
await orchestrator.ExecuteSkillAsync("FileOperations", 
    new Dictionary<string, object> 
    {
        ["operation"] = "read",
        ["path"] = "/path/to/file.txt"
    });
```

Write to a file:
```csharp
await orchestrator.ExecuteSkillAsync("FileOperations", 
    new Dictionary<string, object> 
    {
        ["operation"] = "write",
        ["path"] = "/path/to/output.txt",
        ["content"] = "Hello, ATLAS!"
    });
```

List files in a directory:
```csharp
await orchestrator.ExecuteSkillAsync("FileOperations", 
    new Dictionary<string, object> 
    {
        ["operation"] = "list",
        ["path"] = "/path/to/directory"
    });
```

### ShellCommand Skill

Execute a shell command:
```csharp
await orchestrator.ExecuteSkillAsync("ShellCommand", 
    new Dictionary<string, object> 
    {
        ["command"] = "ls -la"
    });
```

## Cron Tasks

### Creating a Scheduled Task

```csharp
using Atlas.Core.Cron;
using Quartz;

public class DailySummaryJob : IJob
{
    private readonly AtlasOrchestrator _orchestrator;

    public DailySummaryJob(AtlasOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var summary = await _orchestrator.ProcessMessageAsync(
            "Generate a summary of today's activities");
        
        await _orchestrator.SendNotificationAsync(summary, "Telegram");
    }
}

// Register and schedule
var scheduler = host.Services.GetRequiredService<CronScheduler>();
await scheduler.StartAsync();
await scheduler.ScheduleJobAsync<DailySummaryJob>(
    "0 0 9 * * ?", // Every day at 9 AM
    "DailySummary");
```

## Messaging

### Sending Notifications

```csharp
// Send via any available messaging provider
await orchestrator.SendNotificationAsync("Task completed successfully!");

// Send via specific provider
await orchestrator.SendNotificationAsync("Important update", "Telegram");
```

## Memory Usage

### Storing and Retrieving Data

```csharp
var memory = host.Services.GetRequiredService<IMemoryStore>();

// Save data
await memory.SaveAsync("user_preferences", "{\"theme\":\"dark\"}");

// Retrieve data
var prefs = await memory.GetAsync("user_preferences");

// Check existence
if (await memory.ExistsAsync("user_preferences"))
{
    // Data exists
}

// Delete data
await memory.DeleteAsync("user_preferences");
```

## Multi-Provider Workflow

```csharp
// Get responses from multiple providers
var providers = host.Services.GetRequiredService<IEnumerable<ILlmProvider>>();

var question = "What is the meaning of life?";
var responses = new Dictionary<string, string>();

foreach (var provider in providers)
{
    if (await provider.IsAvailableAsync())
    {
        var response = await provider.SendMessageAsync(question);
        responses[provider.Name] = response;
    }
}

// Compare and analyze responses
// ...
```

## Error Handling

```csharp
try
{
    var response = await orchestrator.ProcessMessageAsync("Your message");
    Console.WriteLine(response);
}
catch (InvalidOperationException ex) when (ex.Message.Contains("No LLM provider"))
{
    Console.WriteLine("No LLM providers are available. Please configure at least one.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```
