# ATLAS 🧠⚙️  
Autonomous Task, Learning & Agent System

ATLAS is an ultra-lightweight personal AI assistant built on .NET 10. It orchestrates multiple AI CLIs (Claude, Gemini, Copilot, Codex) through a unified CLI wrapper, supports messaging via Telegram & WhatsApp, and provides tools, skills, custom agents, cron tasks, and persistent memory — enabling automated, agent-driven workflows for developers and power users.

## Features

- **Modern Terminal UI**: VS Code-inspired CLI interface powered by Terminal.Gui
- **Multi-LLM Support**: Unified interface for Claude, Gemini, GitHub Copilot, and Codex
- **CLI Wrapper**: Uses CLIwrap to orchestrate different AI CLI tools
- **Messaging Integration**: Telegram and WhatsApp support for notifications
- **Skills System**: Extensible plugin architecture for custom capabilities
- **Persistent Memory**: File-based storage for conversation context and data
- **Cron Scheduling**: Automated task execution with Quartz.NET
- **Web UI**: Blazor-based dashboard with modern UI components
- **Agent Orchestration**: Coordinate multiple AI agents for complex workflows

## Tech Stack

- **.NET 10**: Latest .NET platform
- **Terminal.Gui**: Modern TUI framework for VS Code-like CLI interface
- **CLIwrap**: Command-line interface wrapper for AI tools
- **Minimal APIs**: Lightweight HTTP APIs
- **Blazor**: Modern web UI framework
- **Quartz.NET**: Enterprise job scheduler
- **Newtonsoft.Json**: JSON serialization

## Project Structure


```
atlas-assistant/
├── src/
│   ├── Atlas.Core/          # Core library with LLM providers, skills, memory
│   └── Atlas.Web/           # Unified Web application and CLI entry point
└── tests/                   # Test projects
```

## Getting Started

### Prerequisites

- .NET 10 SDK
- AI CLI tools (optional, based on providers you want to use):
  - Claude CLI
  - Gemini CLI
  - GitHub CLI (for Copilot)
  - Codex CLI

### Installation

1. Clone the repository:
```bash
git clone https://github.com/evilz/atlas-assistant.git
cd atlas-assistant
```

2. Build the solution:
```bash
dotnet build
```

3. Run the Setup Wizard:
```bash
dotnet run --project src/Atlas.Web -- --setup
```

4. Run the Web UI:
```bash
dotnet run --project src/Atlas.Web
```

## Usage

### CLI Mode

ATLAS now features a modern Terminal.Gui interface inspired by VS Code!

**Terminal.Gui Mode (Default):**
```bash
dotnet run --project src/Atlas.Web -- --cli
```

Features:
- **VS Code-like Interface**: Menu bar, sidebar explorer, and main chat area
- **Explorer Sidebar**: Browse available LLM providers and skills
- **Interactive Chat**: Type messages in the input field and press Enter
- **Keyboard Shortcuts**:
  - `F9`: Access menu bar
  - `Ctrl+Q`: Quit application
  - `Tab`: Navigate between panels

**Simple Console Mode (Fallback):**
```bash
dotnet run --project src/Atlas.Web -- --cli-simple
```

### Web UI

Navigate to `https://localhost:7234` (or `http://localhost:5230`) after running the web project.

## Configuration

### Environment Variables

- `TELEGRAM_BOT_TOKEN`: Telegram bot token for messaging
- `WHATSAPP_API_KEY`: WhatsApp API key for messaging

### Memory Storage

By default, memory is stored in `~/.atlas/memory/`. You can customize this location by configuring the `FileMemoryStore`.

## Skills

ATLAS comes with built-in skills:

- **FileOperations**: Read, write, and list files
- **ShellCommand**: Execute shell commands

### Creating Custom Skills

Implement the `ISkill` interface:

```csharp
public class MyCustomSkill : ISkill
{
    public string Name => "MySkill";
    public string Description => "Description of what my skill does";

    public async Task<string> ExecuteAsync(Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
    {
        // Your implementation
        return "Result";
    }
}
```

Register it in `ServiceCollectionExtensions`:
```csharp
services.AddSingleton<ISkill, MyCustomSkill>();
```

## LLM Providers

ATLAS supports multiple LLM providers through their CLI interfaces:

- **Claude**: Uses `claude` CLI command
- **Gemini**: Uses `gemini` CLI command  
- **GitHub Copilot**: Uses `gh copilot` CLI command
- **Codex**: Uses `codex` CLI command

The orchestrator automatically selects the first available provider or you can specify a preferred one.

## Architecture

### Core Components

- **AtlasOrchestrator**: Main coordination layer for LLMs, skills, and messaging
- **ILlmProvider**: Interface for LLM integrations
- **ISkill**: Interface for extensible capabilities
- **IMemoryStore**: Interface for persistent storage
- **IMessagingProvider**: Interface for messaging platforms
- **CronScheduler**: Task scheduling with cron expressions

## Development

### Building

```bash
dotnet build
```

### Testing

```bash
dotnet test
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

See LICENSE file for details.

## Roadmap

- [ ] Add more LLM providers (OpenAI, Anthropic direct API)
- [ ] Implement conversation history UI
- [ ] Add plugin marketplace
- [ ] Enhance agent workflow capabilities
- [ ] Add voice interface support
- [ ] Implement distributed agent coordination

## Support

For issues and questions, please open an issue on GitHub.
