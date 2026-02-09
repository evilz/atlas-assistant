using Atlas.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Skills;

public class FileOperationsSkill : ISkill
{
    private readonly ILogger<FileOperationsSkill> _logger;

    public string Name => "FileOperations";
    public string Description => "Perform file system operations like read, write, list";

    public FileOperationsSkill(ILogger<FileOperationsSkill> logger)
    {
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
    {
        if (!parameters.TryGetValue("operation", out var operation))
            return "Error: operation parameter is required";

        try
        {
            switch (operation.ToString()?.ToLower())
            {
                case "read":
                    if (parameters.TryGetValue("path", out var readPath))
                    {
                        var content = await File.ReadAllTextAsync(readPath.ToString()!, cancellationToken);
                        return $"File content:\n{content}";
                    }
                    return "Error: path parameter is required for read operation";

                case "write":
                    if (parameters.TryGetValue("path", out var writePath) && parameters.TryGetValue("content", out var writeContent))
                    {
                        await File.WriteAllTextAsync(writePath.ToString()!, writeContent.ToString()!, cancellationToken);
                        return $"File written successfully: {writePath}";
                    }
                    return "Error: path and content parameters are required for write operation";

                case "list":
                    if (parameters.TryGetValue("path", out var listPath))
                    {
                        var files = Directory.GetFiles(listPath.ToString()!);
                        return $"Files:\n{string.Join("\n", files)}";
                    }
                    return "Error: path parameter is required for list operation";

                default:
                    return $"Unknown operation: {operation}";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing file operation: {Operation}", operation);
            return $"Error: {ex.Message}";
        }
    }
}
