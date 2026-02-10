
using System.Threading.Tasks;

namespace Atlas.Core.Abstractions;

public interface IProviderInstaller
{
    string ProviderName { get; }
    Task<bool> IsInstalledAsync();
    Task<bool> InstallAsync();
    Task<bool> IsAuthenticatedAsync();
    Task AuthenticateAsync();
}
