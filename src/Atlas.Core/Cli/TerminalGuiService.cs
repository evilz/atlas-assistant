using Atlas.Core.Abstractions;
using Microsoft.Extensions.Logging;
using Terminal.Gui;

namespace Atlas.Core.Cli;

public class TerminalGuiService
{
    private readonly AtlasOrchestrator _orchestrator;
    private readonly ILogger<TerminalGuiService> _logger;
    private ListView? _providerListView;
    private ListView? _skillListView;
    private TextView? _chatView;
    private TextField? _inputField;
    private Label? _statusLabel;
    private List<string> _conversationHistory = new();

    public TerminalGuiService(AtlasOrchestrator orchestrator, ILogger<TerminalGuiService> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    public void Run()
    {
        Application.Init();

        try
        {
            var top = Application.Top;

            // Create main window
            var win = new Window("ATLAS Assistant")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            // Create menu bar (VS Code style)
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_New Chat", "", () => ClearChat()),
                    new MenuItem ("_Quit", "", () => Application.RequestStop())
                }),
                new MenuBarItem ("_View", new MenuItem [] {
                    new MenuItem ("_Providers", "", () => FocusProviders()),
                    new MenuItem ("_Skills", "", () => FocusSkills()),
                    new MenuItem ("_Chat", "", () => FocusChat())
                }),
                new MenuBarItem ("_Help", new MenuItem [] {
                    new MenuItem ("_About", "", () => ShowAbout())
                })
            });

            top.Add(menu);

            // Create left sidebar (Explorer-like)
            var sidebar = new FrameView("Explorer")
            {
                X = 0,
                Y = 1,
                Width = 30,
                Height = Dim.Fill() - 1
            };

            // Providers section
            var providersLabel = new Label("PROVIDERS")
            {
                X = 1,
                Y = 0,
                ColorScheme = Colors.TopLevel
            };
            sidebar.Add(providersLabel);

            var providers = _orchestrator.GetAvailableProviders().ToList();
            _providerListView = new ListView(providers)
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill() - 2,
                Height = providers.Count + 1
            };
            sidebar.Add(_providerListView);

            // Skills section
            var skillsLabel = new Label("SKILLS")
            {
                X = 1,
                Y = Pos.Bottom(_providerListView) + 1,
                ColorScheme = Colors.TopLevel
            };
            sidebar.Add(skillsLabel);

            var skills = _orchestrator.GetAvailableSkills().ToList();
            _skillListView = new ListView(skills)
            {
                X = 1,
                Y = Pos.Bottom(skillsLabel),
                Width = Dim.Fill() - 2,
                Height = Dim.Fill() - 2
            };
            sidebar.Add(_skillListView);

            win.Add(sidebar);

            // Create main chat area (Editor-like)
            var chatFrame = new FrameView("Chat")
            {
                X = 30,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 4
            };

            _chatView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                WordWrap = true
            };
            chatFrame.Add(_chatView);

            win.Add(chatFrame);

            // Create input area at bottom
            var inputFrame = new FrameView("Message")
            {
                X = 30,
                Y = Pos.Bottom(chatFrame),
                Width = Dim.Fill(),
                Height = 3
            };

            _inputField = new TextField("")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill()
            };

            _inputField.KeyPress += async (e) =>
            {
                if (e.KeyEvent.Key == Key.Enter)
                {
                    await SendMessage();
                    e.Handled = true;
                }
            };

            inputFrame.Add(_inputField);
            win.Add(inputFrame);

            // Create status bar at bottom (VS Code style)
            _statusLabel = new Label("Ready")
            {
                X = 0,
                Y = Pos.AnchorEnd(1),
                Width = Dim.Fill(),
                Height = 1,
                ColorScheme = Colors.TopLevel
            };
            top.Add(_statusLabel);

            top.Add(win);

            // Display welcome message
            DisplayWelcomeMessage();

            Application.Run();
        }
        finally
        {
            Application.Shutdown();
        }
    }

    private void DisplayWelcomeMessage()
    {
        var welcome = @"╔═══════════════════════════════════════════════════════════╗
║              ATLAS Assistant - AI Control System          ║
║                    Powered by Terminal.Gui                ║
╚═══════════════════════════════════════════════════════════╝

Welcome to ATLAS! Your ultra-lightweight AI assistant.

Available Commands:
  - Type your message and press Enter to chat
  - Use Ctrl+Q to quit
  - Use the menu bar (F9) for more options

Providers and Skills are listed in the Explorer sidebar.

";
        _conversationHistory.Add(welcome);
        UpdateChatView();
    }

    private async Task SendMessage()
    {
        if (_inputField == null || _chatView == null || _statusLabel == null)
            return;

        var message = _inputField.Text?.ToString();
        if (string.IsNullOrWhiteSpace(message))
            return;

        // Add user message to history
        _conversationHistory.Add($"\n[USER] {message}");
        UpdateChatView();

        // Clear input field
        _inputField.Text = "";

        // Update status
        _statusLabel.Text = "Processing...";
        Application.Refresh();

        try
        {
            // Process message
            var response = await _orchestrator.ProcessMessageAsync(message);
            
            // Add response to history
            _conversationHistory.Add($"\n[ATLAS] {response}");
            UpdateChatView();

            _statusLabel.Text = "Ready";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
            _conversationHistory.Add($"\n[ERROR] {ex.Message}");
            UpdateChatView();
            _statusLabel.Text = $"Error: {ex.Message}";
        }

        Application.Refresh();
    }

    private void UpdateChatView()
    {
        if (_chatView == null)
            return;

        _chatView.Text = string.Join("\n", _conversationHistory);
        
        // Scroll to bottom
        _chatView.MoveEnd();
    }

    private void ClearChat()
    {
        _conversationHistory.Clear();
        DisplayWelcomeMessage();
        if (_statusLabel != null)
            _statusLabel.Text = "Chat cleared - Ready";
    }

    private void FocusProviders()
    {
        _providerListView?.SetFocus();
    }

    private void FocusSkills()
    {
        _skillListView?.SetFocus();
    }

    private void FocusChat()
    {
        _inputField?.SetFocus();
    }

    private void ShowAbout()
    {
        MessageBox.Query("About ATLAS", 
            "ATLAS Assistant v1.0\n\n" +
            "Autonomous Task, Learning & Agent System\n\n" +
            "An ultra-lightweight AI assistant built on .NET 10\n" +
            "with Terminal.Gui interface.\n\n" +
            "© 2026 ATLAS Team", 
            "OK");
    }
}
