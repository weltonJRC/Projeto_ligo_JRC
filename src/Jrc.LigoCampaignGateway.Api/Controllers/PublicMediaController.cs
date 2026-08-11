using Jrc.LigoCampaignGateway.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Jrc.LigoCampaignGateway.Api.Controllers;

[ApiController]
[Route("public/media")]
public class PublicMediaController : ControllerBase
{
    private readonly IAppDbContext _db;
    private readonly IConfiguration _config;

    public PublicMediaController(IAppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpGet("{assetId}")]
    public IActionResult GetPublicMedia(Guid assetId)
    {
        var asset = _db.MediaAssets.FirstOrDefault(a => a.Id == assetId && a.Active);
        if (asset == null || !System.IO.File.Exists(asset.StoragePath))
        {
            return NotFound("Media asset not found.");
        }

        Response.Headers.Append("X-Content-Type-Options", "nosniff");
        return PhysicalFile(asset.StoragePath, asset.ContentType);
    }
}
