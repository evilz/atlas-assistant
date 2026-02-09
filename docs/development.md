# ATLAS Development Guide

This guide is for developers who want to contribute to ATLAS or extend its functionality.

## Development Setup

### Prerequisites

- .NET 10 SDK
- Git
- IDE (Visual Studio, VS Code, or Rider recommended)
- Optional: AI CLI tools for testing (claude, gemini, gh copilot, codex)

### Getting Started

1. Clone the repository:
```bash
git clone https://github.com/evilz/atlas-assistant.git
cd atlas-assistant
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the solution:
```bash
dotnet build
```

4. Run tests:
```bash
dotnet test
```

## Project Structure

```
atlas-assistant/
├── src/
│   ├── Atlas.Core/          # Core library
│   │   ├── Abstractions/    # Interfaces
│   │   ├── Providers/       # LLM provider implementations
│   │   ├── Skills/          # Built-in skills
│   │   ├── Memory/          # Memory storage
│   │   ├── Messaging/       # Messaging providers
│   │   ├── Cron/            # Cron scheduler
│   │   └── Models/          # Data models
│   ├── Atlas.CLI/           # Command-line interface
│   └── Atlas.Web/           # Blazor web application
├── tests/
│   └── Atlas.Core.Tests/    # Unit tests
└── docs/                    # Documentation
```

## Adding a New LLM Provider

1. Create a new provider class implementing `ILlmProvider`:

```csharp
using Atlas.Core.Abstractions;
using Atlas.Core.Models;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Providers;

public class MyLlmProvider : ILlmProvider
{
    private readonly ILogger<MyLlmProvider> _logger;

    public string Name => "MyLLM";

    public MyLlmProvider(ILogger<MyLlmProvider> logger)
    {
        _logger = logger;
    }

    public async Task<string> SendMessageAsync(
        string message, 
        ConversationContext? context = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await Cli.Wrap("myllm")
                .WithArguments(message)
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync(cancellationToken);

            return result.StandardOutput;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling MyLLM CLI");
            throw;
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await Cli.Wrap("which")
                .WithArguments("myllm")
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync(cancellationToken);
            
            return result.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
```

2. Register in `ServiceCollectionExtensions.cs`:

```csharp
services.AddSingleton<ILlmProvider, MyLlmProvider>();
```

## Adding a New Skill

1. Create a skill class implementing `ISkill`:

```csharp
using Atlas.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Skills;

public class MySkill : ISkill
{
    private readonly ILogger<MySkill> _logger;

    public string Name => "MySkill";
    public string Description => "Description of what this skill does";

    public MySkill(ILogger<MySkill> logger)
    {
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(
        Dictionary<string, object> parameters, 
        CancellationToken cancellationToken = default)
    {
        // Implementation
        return "Result";
    }
}
```

2. Register in `ServiceCollectionExtensions.cs`:

```csharp
services.AddSingleton<ISkill, MySkill>();
```

## Testing

### Writing Unit Tests

Create tests in the `tests/Atlas.Core.Tests` project:

```csharp
using Xunit;

namespace Atlas.Core.Tests;

public class MySkillTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnExpectedResult()
    {
        // Arrange
        var skill = new MySkill(NullLogger<MySkill>.Instance);
        var parameters = new Dictionary<string, object>
        {
            ["param1"] = "value1"
        };

        // Act
        var result = await skill.ExecuteAsync(parameters);

        // Assert
        Assert.Contains("expected", result);
    }
}
```

### Running Tests

Run all tests:
```bash
dotnet test
```

Run specific test class:
```bash
dotnet test --filter ClassName=MySkillTests
```

Run with coverage:
```bash
dotnet test /p:CollectCoverage=true
```

## Building and Running

### CLI Application

Build:
```bash
dotnet build src/Atlas.CLI
```

Run:
```bash
dotnet run --project src/Atlas.CLI
```

### Web Application

Build:
```bash
dotnet build src/Atlas.Web
```

Run:
```bash
dotnet run --project src/Atlas.Web
```

Navigate to `https://localhost:7234`

## Code Style

- Follow standard C# coding conventions
- Use async/await for asynchronous operations
- Add XML documentation comments for public APIs
- Use dependency injection
- Keep methods focused and single-purpose
- Handle exceptions appropriately

## Debugging

### CLI Application

In VS Code, use this launch configuration:

```json
{
    "name": ".NET Core Launch (console)",
    "type": "coreclr",
    "request": "launch",
    "preLaunchTask": "build",
    "program": "${workspaceFolder}/src/Atlas.CLI/bin/Debug/net10.0/Atlas.CLI.dll",
    "args": [],
    "cwd": "${workspaceFolder}/src/Atlas.CLI",
    "stopAtEntry": false
}
```

### Web Application

```json
{
    "name": ".NET Core Launch (web)",
    "type": "coreclr",
    "request": "launch",
    "preLaunchTask": "build",
    "program": "${workspaceFolder}/src/Atlas.Web/bin/Debug/net10.0/Atlas.Web.dll",
    "args": [],
    "cwd": "${workspaceFolder}/src/Atlas.Web",
    "stopAtEntry": false,
    "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
    },
    "sourceFileMap": {
        "/Views": "${workspaceFolder}/Views"
    }
}
```

## Performance Considerations

- Use `IAsyncEnumerable<T>` for streaming responses
- Implement caching for expensive operations
- Use `ConfigureAwait(false)` in library code
- Profile memory usage for long-running operations
- Consider using `IHostedService` for background tasks

## Security Best Practices

- Never commit API keys or tokens
- Use environment variables for sensitive configuration
- Validate all user inputs
- Sanitize file paths to prevent directory traversal
- Use secure communication protocols (HTTPS)
- Implement rate limiting for API calls

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Ensure all tests pass
6. Submit a pull request

## Release Process

1. Update version in project files
2. Update CHANGELOG.md
3. Run full test suite
4. Build release packages
5. Tag release in Git
6. Publish to NuGet (if applicable)

## Troubleshooting

### Build Issues

Clear build artifacts:
```bash
dotnet clean
dotnet restore
dotnet build
```

### Test Failures

Run tests verbosely:
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Runtime Issues

Enable detailed logging:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

## Additional Resources

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [CLIwrap Documentation](https://github.com/Tyrrrz/CliWrap)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor/)
- [Quartz.NET Documentation](https://www.quartz-scheduler.net/)
