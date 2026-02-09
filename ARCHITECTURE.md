# ATLAS Architecture

## System Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         ATLAS ASSISTANT                          │
└─────────────────────────────────────────────────────────────────┘

┌──────────────────┐                              ┌──────────────────┐
│   Atlas.CLI      │                              │   Atlas.Web      │
│  (Console App)   │                              │  (Blazor Server) │
│                  │                              │                  │
│  - Interactive   │                              │  - Dashboard     │
│  - Single Cmd    │                              │  - Chat UI       │
│  - Logging       │                              │  - Config UI     │
└────────┬─────────┘                              └────────┬─────────┘
         │                                                 │
         └────────────────────┬────────────────────────────┘
                              │
                              ▼
         ┌────────────────────────────────────────────┐
         │         Atlas.Core (Library)               │
         │  ┌──────────────────────────────────────┐  │
         │  │     AtlasOrchestrator                │  │
         │  │  (Central Coordination Layer)        │  │
         │  └──────────────────────────────────────┘  │
         │                    │                        │
         │  ┌─────────────────┼─────────────────┐     │
         │  │                 │                 │     │
         │  ▼                 ▼                 ▼     │
         │ ┌───────┐    ┌─────────┐     ┌─────────┐ │
         │ │  LLM  │    │ Skills  │     │Messaging│ │
         │ │Providr│    │ System  │     │  System │ │
         │ └───┬───┘    └────┬────┘     └────┬────┘ │
         │     │             │                │      │
         └─────┼─────────────┼────────────────┼──────┘
               │             │                │
               │             │                │
         ┌─────┴─────┐ ┌─────┴─────┐   ┌─────┴─────┐
         │           │ │           │   │           │
         ▼           ▼ ▼           ▼   ▼           ▼
    ┌────────┐  ┌────────┐   ┌─────────┐  ┌──────────┐
    │ Claude │  │ Gemini │   │  File   │  │ Telegram │
    │  CLI   │  │  CLI   │   │Operations  │   Bot    │
    └────────┘  └────────┘   └─────────┘  └──────────┘
    ┌────────┐  ┌────────┐   ┌─────────┐  ┌──────────┐
    │Copilot │  │ Codex  │   │  Shell  │  │ WhatsApp │
    │  CLI   │  │  CLI   │   │ Command│  │   API    │
    └────────┘  └────────┘   └─────────┘  └──────────┘
```

## Component Details

### 1. Presentation Layer

#### Atlas.CLI
- Entry point for command-line usage
- Interactive REPL mode
- Single-command execution
- Provider and skill listing

#### Atlas.Web
- Blazor Server application
- Real-time chat interface
- Dashboard with system stats
- Provider selection UI

### 2. Business Logic Layer (Atlas.Core)

#### AtlasOrchestrator
Central coordination service that:
- Routes messages to appropriate LLM providers
- Executes skills with parameters
- Sends notifications via messaging providers
- Manages provider availability

#### LLM Providers
Interface: `ILlmProvider`
- **ClaudeProvider** - Anthropic Claude
- **GeminiProvider** - Google Gemini
- **CopilotProvider** - GitHub Copilot
- **CodexProvider** - OpenAI Codex

Each provider:
- Wraps CLI using CLIwrap
- Checks availability via `which` command
- Returns responses or throws exceptions

#### Skills System
Interface: `ISkill`
- **FileOperationsSkill** - File I/O operations
- **ShellCommandSkill** - Execute shell commands
- Extensible via plugin pattern

#### Messaging System
Interface: `IMessagingProvider`
- **TelegramProvider** - Telegram Bot API
- **WhatsAppProvider** - WhatsApp Business API
- Environment variable configuration

#### Memory System
Interface: `IMemoryStore`
- **FileMemoryStore** - Persistent file-based storage
- JSON serialization
- Key-value storage in ~/.atlas/memory

#### Cron System
- **CronScheduler** - Quartz.NET wrapper
- Schedule jobs with cron expressions
- Background task execution

### 3. Data Layer

#### Models
- **Message** - Chat message representation
- **ConversationContext** - Conversation metadata
- Configuration POCOs

#### Storage
- File-based JSON storage
- Environment variables
- appsettings.json

## Data Flow

### Chat Message Flow
```
User Input → CLI/Web → AtlasOrchestrator → LLM Provider → CLI Tool → Response
                                    ↓
                            Memory Store (optional)
```

### Skill Execution Flow
```
Skill Request → AtlasOrchestrator → Skill Implementation → Result
                                    ↓
                            Logging & Error Handling
```

### Notification Flow
```
Trigger → AtlasOrchestrator → Messaging Provider → External API → User
```

## Dependency Injection Container

```
ServiceCollection
├── Singletons
│   ├── ILlmProvider (Claude, Gemini, Copilot, Codex)
│   ├── IMessagingProvider (Telegram, WhatsApp)
│   ├── ISkill (FileOperations, ShellCommand)
│   ├── IMemoryStore (FileMemoryStore)
│   ├── AtlasOrchestrator
│   └── CronScheduler
├── Scoped
│   └── (Web UI components)
└── Transient
    └── (Loggers)
```

## Configuration Flow

```
appsettings.json
       ↓
Environment Variables (override)
       ↓
IConfiguration
       ↓
Services (via DI)
```

## Error Handling Strategy

```
Exception → Logged → Graceful Response → User Notification
                ↓
         (Critical: Rethrow)
         (Expected: Handle)
```

## Extension Points

1. **New LLM Provider**: Implement `ILlmProvider`
2. **New Skill**: Implement `ISkill`
3. **New Messaging**: Implement `IMessagingProvider`
4. **New Storage**: Implement `IMemoryStore`
5. **Custom UI**: Reference Atlas.Core

## Security Considerations

- API keys in environment variables
- Input validation in skills
- Path sanitization in FileOperations
- Command injection prevention in ShellCommand
- HTTPS for web interface
- No secrets in source code

## Performance Characteristics

- **Async I/O**: All network/file operations
- **Lazy Loading**: Providers checked on-demand
- **Minimal Memory**: File-based storage
- **Fast Startup**: Lightweight DI container
- **Scalability**: Stateless services (except memory)

## Technology Choices Rationale

- **.NET 10**: Latest features, performance, long-term support
- **CLIwrap**: Reliable CLI wrapping, async support
- **Quartz.NET**: Industry-standard scheduler
- **Blazor Server**: Real-time updates, C# throughout
- **File-based Memory**: Simple, no external dependencies
- **Dependency Injection**: Testability, maintainability
