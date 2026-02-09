using Atlas.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace Atlas.Core.Memory;

public class FileMemoryStore : IMemoryStore
{
    private readonly string _storePath;
    private readonly ILogger<FileMemoryStore> _logger;

    public FileMemoryStore(ILogger<FileMemoryStore> logger, string? storePath = null)
    {
        _logger = logger;
        _storePath = storePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".atlas",
            "memory"
        );
        
        Directory.CreateDirectory(_storePath);
    }

    public async Task SaveAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = GetFilePath(key);
            await File.WriteAllTextAsync(filePath, value, cancellationToken);
            _logger.LogDebug("Saved memory key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving memory key: {Key}", key);
            throw;
        }
    }

    public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = GetFilePath(key);
            if (!File.Exists(filePath))
                return null;

            return await File.ReadAllTextAsync(filePath, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading memory key: {Key}", key);
            throw;
        }
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(key);
        return Task.FromResult(File.Exists(filePath));
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = GetFilePath(key);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogDebug("Deleted memory key: {Key}", key);
            }
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting memory key: {Key}", key);
            throw;
        }
    }

    private string GetFilePath(string key)
    {
        var safeKey = string.Join("_", key.Split(Path.GetInvalidFileNameChars()));
        return Path.Combine(_storePath, $"{safeKey}.json");
    }
}
