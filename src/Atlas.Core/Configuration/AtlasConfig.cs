
using Tomlyn.Model;

namespace Atlas.Core.Configuration;

public class AtlasConfig
{
    public Dictionary<string, ProviderConfig> Providers { get; set; } = new();
}

public class ProviderConfig
{
    public bool Enabled { get; set; }
    public Dictionary<string, string> Settings { get; set; } = new();
}
