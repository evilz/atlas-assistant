# Example: Custom Skill Implementation

This example shows how to create a custom skill for ATLAS.

## Creating a Custom Skill

```csharp
using Atlas.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace MyProject.Skills;

public class WeatherSkill : ISkill
{
    private readonly ILogger<WeatherSkill> _logger;

    public string Name => "Weather";
    public string Description => "Get current weather information for a location";

    public WeatherSkill(ILogger<WeatherSkill> logger)
    {
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(
        Dictionary<string, object> parameters, 
        CancellationToken cancellationToken = default)
    {
        if (!parameters.TryGetValue("location", out var location))
            return "Error: location parameter is required";

        try
        {
            // Your implementation here
            // For example, call a weather API
            var weatherData = await GetWeatherDataAsync(location.ToString()!, cancellationToken);
            
            return $"Weather in {location}: {weatherData}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting weather for {Location}", location);
            return $"Error: {ex.Message}";
        }
    }

    private async Task<string> GetWeatherDataAsync(string location, CancellationToken cancellationToken)
    {
        // Implementation details
        await Task.Delay(100, cancellationToken);
        return "Sunny, 72°F";
    }
}
```

## Registering the Skill

Add to `ServiceCollectionExtensions.cs`:

```csharp
services.AddSingleton<ISkill, WeatherSkill>();
```

## Using the Skill

From CLI or Web UI:
```csharp
var result = await orchestrator.ExecuteSkillAsync("Weather", 
    new Dictionary<string, object> 
    { 
        ["location"] = "New York" 
    });
```
