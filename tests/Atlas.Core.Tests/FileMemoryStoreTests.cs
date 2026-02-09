using Atlas.Core.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace Atlas.Core.Tests;

public class FileMemoryStoreTests : IDisposable
{
    private readonly string _testPath;

    public FileMemoryStoreTests()
    {
        _testPath = Path.Combine(Path.GetTempPath(), "atlas-tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testPath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testPath))
        {
            Directory.Delete(_testPath, true);
        }
    }

    [Fact]
    public async Task SaveAsync_ShouldSaveValue()
    {
        // Arrange
        var store = new FileMemoryStore(NullLogger<FileMemoryStore>.Instance, _testPath);
        var key = "test-key";
        var value = "test-value";

        // Act
        await store.SaveAsync(key, value);

        // Assert
        var result = await store.GetAsync(key);
        Assert.Equal(value, result);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNullForNonExistentKey()
    {
        // Arrange
        var store = new FileMemoryStore(NullLogger<FileMemoryStore>.Instance, _testPath);

        // Act
        var result = await store.GetAsync("non-existent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrueForExistingKey()
    {
        // Arrange
        var store = new FileMemoryStore(NullLogger<FileMemoryStore>.Instance, _testPath);
        var key = "existing-key";
        await store.SaveAsync(key, "value");

        // Act
        var exists = await store.ExistsAsync(key);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalseForNonExistentKey()
    {
        // Arrange
        var store = new FileMemoryStore(NullLogger<FileMemoryStore>.Instance, _testPath);

        // Act
        var exists = await store.ExistsAsync("non-existent");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveKey()
    {
        // Arrange
        var store = new FileMemoryStore(NullLogger<FileMemoryStore>.Instance, _testPath);
        var key = "key-to-delete";
        await store.SaveAsync(key, "value");

        // Act
        await store.DeleteAsync(key);

        // Assert
        var exists = await store.ExistsAsync(key);
        Assert.False(exists);
    }
}
