using System.Security.Cryptography;
using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Jrc.LigoCampaignGateway.Api.Controllers;

[ApiController]
[Route("api/v1/media")]
public class MediaAssetsController : ControllerBase
{
    private readonly IAppDbContext _db;
    private readonly IMediaAssetStorage _storage;

    public MediaAssetsController(IAppDbContext db, IMediaAssetStorage storage)
    {
        _db = db;
        _storage = storage;
    }

    [HttpPost("assets")]
    public async Task<IActionResult> UploadAsset(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("File is empty.");
        if (file.Length > 5 * 1024 * 1024) return BadRequest("File size exceeds 5MB limit.");

        var allowedMimes = new[] { "image/png", "image/jpeg", "image/jpg" };
        if (!allowedMimes.Contains(file.ContentType.ToLowerInvariant()))
        {
            return BadRequest("Only PNG and JPEG images are allowed.");
        }

        var assetId = Guid.NewGuid();

        using var sha256 = SHA256.Create();
        using var stream = file.OpenReadStream();
        var hashBytes = await sha256.ComputeHashAsync(stream, HttpContext.RequestAborted);
        var hash = Convert.ToHexString(hashBytes);

        stream.Position = 0;
        var storagePath = await _storage.SaveAssetAsync(stream, file.FileName, HttpContext.RequestAborted);

        var asset = new MediaAsset
        {
            Id = assetId,
            OriginalFileName = file.FileName,
            StoredFileName = Path.GetFileName(storagePath),
            ContentType = file.ContentType,
            SizeBytes = file.Length,
            Sha256 = hash,
            StorageProvider = "LocalDisk",
            StoragePath = storagePath,
            PublicUrl = $"/public/media/{assetId}"
        };

        await _db.AddMediaAssetAsync(asset, HttpContext.RequestAborted);
        await _db.SaveChangesAsync(HttpContext.RequestAborted);

        return Ok(asset);
    }
}
