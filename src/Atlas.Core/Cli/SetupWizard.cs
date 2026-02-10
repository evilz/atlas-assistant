
using Atlas.Core.Abstractions;
using Atlas.Core.Configuration;
using Atlas.Core.Installers;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Cli;

public class SetupWizard
{
    private readonly ProviderInstallerFactory _installerFactory;
    private readonly ConfigurationService _configService;
    private readonly ILogger<SetupWizard> _logger;

    public SetupWizard(
        ProviderInstallerFactory installerFactory, 
        ConfigurationService configService,
        ILogger<SetupWizard> logger)
    {
        _installerFactory = installerFactory;
        _configService = configService;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        // Ensure config is loaded
        await _configService.LoadAsync();

        Console.WriteLine("\n=================================================");
        Console.WriteLine("   ATLAS AI Provider Setup Wizard");
        Console.WriteLine("=================================================\n");
        Console.WriteLine("This wizard will help you set up your AI providers.");

        var installers = _installerFactory.GetAllInstallers().ToList();

        while (true)
        {
            Console.WriteLine("\nAvailable Providers:");
            for (int i = 0; i < installers.Count; i++)
            {
                var installer = installers[i];
                var installed = await installer.IsInstalledAsync();
                
                // Check config state
                string status = "[Not Installed]";
                if (installed)
                {
                    if (await installer.IsAuthenticatedAsync())
                    {
                        status = "[Enabled]";
                    }
                    else
                    {
                        status = "[Installed]";
                    }
                }
                
                // Check if enabled in our local config
                if (_configService.CurrentConfig.Providers.TryGetValue(installer.ProviderName, out var pConfig) && pConfig.Enabled)
                {
                    status += " (Configured)";
                }
                
                Console.WriteLine($"{i + 1}. {installer.ProviderName} {status}");
            }
            Console.WriteLine("0. Finish Setup");

            Console.Write("\nSelect a provider to install/configure (0-4): ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || input == "0")
            {
                break;
            }

            if (int.TryParse(input, out int selection) && selection > 0 && selection <= installers.Count)
            {
                var selectedInstaller = installers[selection - 1];
                await ProcessProviderAsync(selectedInstaller);
            }
            else
            {
                Console.WriteLine("Invalid selection. Please try again.");
            }
        }

        Console.WriteLine("\nSetup complete! Saving configuration...");
        await _configService.SaveAsync();
        Console.WriteLine("Configuration saved.");
    }

    private async Task ProcessProviderAsync(IProviderInstaller installer)
    {
        Console.WriteLine($"\n--- Setting up {installer.ProviderName} ---");

        bool isInstalled = await installer.IsInstalledAsync();

        if (!isInstalled)
        {
            Console.WriteLine($"{installer.ProviderName} is not installed.");
            Console.Write("Do you want to download and install it? (y/n): ");
            var response = Console.ReadLine();

            if (response?.Trim().ToLower() == "y")
            {
                bool success = await installer.InstallAsync();
                if (success)
                {
                    Console.WriteLine("Installation successful!");
                    isInstalled = true;
                }
                else
                {
                    Console.WriteLine("Installation failed. Please check the logs for details.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Skipping installation.");
                return;
            }
        }
        else
        {
            Console.WriteLine($"{installer.ProviderName} is already installed.");
        }

        if (isInstalled)
        {
            Console.Write("Do you want to launch authentication/configuration? (y/n): ");
            var response = Console.ReadLine();
            if (response?.Trim().ToLower() == "y")
            {
                await installer.AuthenticateAsync();
                
                // Update our configuration to mark it as enabled
                if (!_configService.CurrentConfig.Providers.ContainsKey(installer.ProviderName))
                {
                    _configService.CurrentConfig.Providers[installer.ProviderName] = new ProviderConfig();
                }
                _configService.CurrentConfig.Providers[installer.ProviderName].Enabled = true;
                Console.WriteLine($"Marked {installer.ProviderName} as enabled in configuration.");
            }
        }
    }
}
