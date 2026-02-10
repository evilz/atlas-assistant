
using Atlas.Core.Providers;
using CliWrap;
using CliWrap.Buffered;

namespace Atlas.Core.Helpers;

public class NodeJsDetector
{
    public static async Task<bool> IsNodeInstalledAsync()
    {
        return await CommandHelper.IsCommandAvailableAsync("node") && 
               await CommandHelper.IsCommandAvailableAsync("npm");
    }

    public static async Task<string?> GetNodeVersionAsync()
    {
        try
        {
            var result = await Cli.Wrap("node")
                .WithArguments("--version")
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync();

            return result.ExitCode == 0 ? result.StandardOutput.Trim() : null;
        }
        catch
        {
            return null;
        }
    }
}
