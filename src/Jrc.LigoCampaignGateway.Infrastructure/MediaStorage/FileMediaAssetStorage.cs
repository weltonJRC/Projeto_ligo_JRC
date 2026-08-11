using Jrc.LigoCampaignGateway.Application.Abstractions;
using Microsoft.Extensions.Hosting;

namespace Jrc.LigoCampaignGateway.Infrastructure.MediaStorage;

public class FileMediaAssetStorage : IMediaAssetStorage
{
    private readonly string _storageFolder;

    public FileMediaAssetStorage(IHostEnvironment env)
    {
        _storageFolder = Path.Combine(env.ContentRootPath, "Storage", "Media");
        Directory.CreateDirectory(_storageFolder);
    }

    public async Task<string> SaveAssetAsync(Stream stream, string originalFileName, CancellationToken ct = default)
    {
        var ext = Path.GetExtension(originalFileName);
        var storedName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(_storageFolder, storedName);

        using var fileStream = File.Create(fullPath);
        await stream.CopyToAsync(fileStream, ct);

        return fullPath;
    }

    public Task<Stream> ReadAssetAsync(string storagePath, CancellationToken ct = default)
    {
        if (!File.Exists(storagePath))
        {
            throw new FileNotFoundException("Media file not found on disk.", storagePath);
        }

        Stream stream = File.OpenRead(storagePath);
        return Task.FromResult(stream);
    }
}
