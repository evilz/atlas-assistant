
using Atlas.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Core.Installers;

public class ProviderInstallerFactory
{
    private readonly IEnumerable<IProviderInstaller> _installers;

    public ProviderInstallerFactory(IEnumerable<IProviderInstaller> installers)
    {
        _installers = installers;
    }

    public IProviderInstaller? GetInstaller(string providerName)
    {
        return _installers.FirstOrDefault(i => i.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<IProviderInstaller> GetAllInstallers()
    {
        return _installers;
    }
}
