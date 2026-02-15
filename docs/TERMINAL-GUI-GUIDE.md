# Terminal.Gui CLI Guide

## Overview

ATLAS features a modern Terminal User Interface (TUI) powered by Terminal.Gui, providing a VS Code-like experience directly in your terminal.

## Starting the Terminal.Gui CLI

```bash
dotnet run --project src/Atlas.Web -- --cli
```

## Interface Layout

The Terminal.Gui interface is divided into four main areas:

```
┌─────────────────────────────────────────────────────────┐
│ File  View  Help                    [Menu Bar]          │
├───────────────┬─────────────────────────────────────────┤
│ EXPLORER      │ CHAT                                    │
│               │                                         │
│ PROVIDERS     │ [Conversation History]                  │
│  - Codex      │                                         │
│  - Claude     │                                         │
│  - Gemini     │                                         │
│  - Copilot    │                                         │
│               │                                         │
│ SKILLS        │                                         │
│  - FileOps    │                                         │
│  - ShellCmd   │                                         │
│               │                                         │
│               ├─────────────────────────────────────────┤
│               │ MESSAGE                                 │
│               │ [Type your message here]                │
├───────────────┴─────────────────────────────────────────┤
│ Ready                               [Status Bar]        │
└─────────────────────────────────────────────────────────┘
```

## Components

### 1. Menu Bar (Top)

Access various commands and features:

- **File**
  - New Chat: Clear the conversation history
  - Quit: Exit the application

- **View**
  - Providers: Focus on the providers list
  - Skills: Focus on the skills list
  - Chat: Focus on the message input

- **Help**
  - About: Show information about ATLAS

**Shortcut**: Press `F9` to access the menu bar

### 2. Explorer Sidebar (Left)

Browse available resources:

- **PROVIDERS**: Lists all configured LLM providers (Claude, Gemini, Copilot, Codex)
- **SKILLS**: Displays available skills and their descriptions

### 3. Chat Area (Right - Main)

The main conversation window showing:
- Welcome message
- Your messages (prefixed with [USER])
- ATLAS responses (prefixed with [ATLAS])
- Error messages (if any)

The chat area automatically scrolls to show the latest messages.

### 4. Message Input (Bottom Right)

Type your messages here and press `Enter` to send them to ATLAS.

### 5. Status Bar (Bottom)

Shows the current status:
- "Ready": Waiting for input
- "Processing...": ATLAS is thinking
- Error messages (if applicable)

## Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `Enter` | Send message |
| `F9` | Open menu bar |
| `Tab` | Navigate between panels |
| `Ctrl+Q` | Quit application |
| `Esc` | Close menu/dialog |

## Usage Examples

### 1. Starting a Chat Session

1. Start the CLI: `dotnet run --project src/Atlas.Web -- --cli`
2. Type your message in the Message input field
3. Press `Enter` to send
4. Wait for ATLAS to respond

### 2. Viewing Available Providers

Check the Explorer sidebar under "PROVIDERS" to see which LLM providers are available in your system.

### 3. Browsing Skills

The "SKILLS" section in the Explorer shows all available skills that ATLAS can use to help you.

### 4. Clearing the Chat

- Press `F9` to open the menu
- Select `File` → `New Chat`
- Or use keyboard: `F9`, `↓`, `Enter`

## Tips

1. **Navigation**: Use `Tab` to move focus between the Explorer, Chat, and Message input areas
2. **Menu Access**: Press `F9` anytime to access the menu bar
3. **Scrolling**: The chat area automatically scrolls to show new messages
4. **Long Messages**: The chat area supports word wrap for long messages

## Fallback to Simple CLI

If you encounter issues with Terminal.Gui or prefer a simpler interface, use:

```bash
dotnet run --project src/Atlas.Web -- --cli-simple
```

This starts the original console-based CLI with a basic prompt interface.

## Troubleshooting

### Terminal.Gui doesn't display correctly

- Ensure your terminal supports 256 colors
- Try resizing your terminal window
- Use the `--cli-simple` flag as a fallback

### Application freezes

- Press `Ctrl+C` to force quit
- Check if a message is being processed (see status bar)

### Cannot type in Message field

- Press `Tab` to focus on the Message input field
- Check if a dialog is open (press `Esc` to close)

## Comparison: Terminal.Gui vs Simple CLI

| Feature | Terminal.Gui | Simple CLI |
|---------|--------------|------------|
| Interface | VS Code-like panels | Basic prompt |
| Navigation | Mouse + Keyboard | Keyboard only |
| Provider/Skills View | Visual sidebar | Startup list only |
| Menu System | Yes | No |
| Status Bar | Yes | No |
| Message History | Scrollable view | Console output |
| User Experience | Modern, intuitive | Minimal, fast |

## VS Code-like Features

ATLAS Terminal.Gui CLI takes inspiration from VS Code:

1. **Explorer Sidebar**: Similar to VS Code's file explorer, showing providers and skills
2. **Menu Bar**: File, View, and Help menus like VS Code
3. **Status Bar**: Shows current status at the bottom
4. **Panel Layout**: Separated areas for different functions
5. **Keyboard Navigation**: Familiar shortcuts for developers

Enjoy your enhanced ATLAS experience! 🚀
