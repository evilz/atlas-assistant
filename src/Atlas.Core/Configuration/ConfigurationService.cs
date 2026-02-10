
using System.Text;
using Microsoft.Extensions.Logging;
using Tomlyn;
using Tomlyn.Model;

namespace Atlas.Core.Configuration;

public class ConfigurationService
{
    private readonly ILogger<ConfigurationService> _logger;
    private readonly string _configPath;
    
    public AtlasConfig CurrentConfig { get; private set; } = new();

    public ConfigurationService(ILogger<ConfigurationService> logger)
    {
        _logger = logger;
        _configPath = Path.Combine(Environment.CurrentDirectory, "atlas.toml");
    }

    public async Task LoadAsync()
    {
        if (!File.Exists(_configPath))
        {
            _logger.LogInformation("No configuration file found at {Path}. Using defaults.", _configPath);
            return;
        }

        try
        {
            var toml = await File.ReadAllTextAsync(_configPath);
            CurrentConfig = Toml.ToModel<AtlasConfig>(toml);
            _logger.LogInformation("Configuration loaded from {Path}", _configPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load configuration from {Path}", _configPath);
            // Fallback to defaults or re-throw if critical
        }
    }

    public async Task SaveAsync()
    {
        try
        {
            var toml = Toml.FromModel(CurrentConfig);
            await File.WriteAllTextAsync(_configPath, toml, Encoding.UTF8);
            _logger.LogInformation("Configuration saved to {Path}", _configPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save configuration to {Path}", _configPath);
        }
    }
}
