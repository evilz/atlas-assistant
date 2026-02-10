
using Atlas.Core.Installers;
using Atlas.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace Atlas.CLI;

public class ProviderWizard
{
    private readonly ProviderInstallerFactory _installerFactory;
    private readonly ILogger<ProviderWizard> _logger;

    public ProviderWizard(ProviderInstallerFactory installerFactory, ILogger<ProviderWizard> logger)
    {
        _installerFactory = installerFactory;
        _logger = logger;
    }

    public async Task RunAsync()
    {
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

        Console.WriteLine("\nSetup complete! You can run this wizard again anytime.");
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
            }
        }
    }
}
