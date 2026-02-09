using CliWrap;
using CliWrap.Buffered;

namespace Atlas.Core.Providers;

internal static class CommandHelper
{
    /// <summary>
    /// Checks if a command is available on the system (cross-platform)
    /// </summary>
    public static async Task<bool> IsCommandAvailableAsync(string commandName, CancellationToken cancellationToken = default)
    {
        try
        {
            // Use 'where' on Windows, 'which' on Unix-like systems
            var checkCommand = OperatingSystem.IsWindows() ? "where" : "which";
            
            var result = await Cli.Wrap(checkCommand)
                .WithArguments(commandName)
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync(cancellationToken);
            
            return result.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
