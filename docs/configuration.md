# ATLAS Configuration Guide

This guide explains how to configure ATLAS Assistant.

## Configuration Files

ATLAS uses standard .NET configuration files:
- `appsettings.json` - Default configuration
- `appsettings.Development.json` - Development overrides
- Environment variables - Runtime overrides

## LLM Provider Configuration

Configure which LLM providers to use and their settings:

```json
{
  "Atlas": {
    "LlmProviders": {
      "PreferredProvider": "Claude",
      "Claude": {
        "CliCommand": "claude",
        "Enabled": true
      }
    }
  }
}
```

### Supported Providers

1. **Claude** - Anthropic's Claude via CLI
   - Requires: `claude` CLI tool
   - Command: `claude`

2. **Gemini** - Google's Gemini via CLI
   - Requires: `gemini` CLI tool
   - Command: `gemini`

3. **GitHub Copilot** - GitHub Copilot CLI
   - Requires: `gh` CLI tool with Copilot extension
   - Command: `gh copilot`

4. **Codex** - OpenAI Codex via CLI
   - Requires: `codex` CLI tool
   - Command: `codex`

## Messaging Configuration

### Telegram

```json
{
  "Atlas": {
    "Messaging": {
      "Telegram": {
        "Enabled": true,
        "BotToken": "YOUR_BOT_TOKEN"
      }
    }
  }
}
```

Or use environment variable:
```bash
export TELEGRAM_BOT_TOKEN=your_token_here
```

### WhatsApp

```json
{
  "Atlas": {
    "Messaging": {
      "WhatsApp": {
        "Enabled": true,
        "ApiKey": "YOUR_API_KEY"
      }
    }
  }
}
```

Or use environment variable:
```bash
export WHATSAPP_API_KEY=your_key_here
```

## Memory Storage

Configure where conversation history and data is stored:

```json
{
  "Atlas": {
    "Memory": {
      "StorePath": "~/.atlas/memory"
    }
  }
}
```

Default location: `~/.atlas/memory/`

## Cron Jobs

Configure automated tasks:

```json
{
  "Atlas": {
    "CronJobs": [
      {
        "Name": "DailySummary",
        "Schedule": "0 0 9 * * ?",
        "Description": "Send daily summary at 9 AM",
        "Enabled": true
      }
    ]
  }
}
```

### Cron Expression Format

Uses Quartz.NET cron format:
```
* * * * * ?
│ │ │ │ │ │
│ │ │ │ │ └─ Day of week (0-7, both 0 and 7 are Sunday)
│ │ │ │ └─── Month (1-12)
│ │ │ └───── Day of month (1-31)
│ │ └─────── Hour (0-23)
│ └───────── Minute (0-59)
└─────────── Second (0-59)
```

Examples:
- `0 0 9 * * ?` - Every day at 9:00 AM
- `0 */15 * * * ?` - Every 15 minutes
- `0 0 0 ? * MON-FRI` - Every weekday at midnight

## Logging

Configure logging levels:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Atlas.Core": "Debug",
      "Microsoft": "Warning"
    }
  }
}
```
