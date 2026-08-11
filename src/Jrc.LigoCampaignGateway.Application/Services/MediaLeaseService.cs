using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Application.Models;
using Jrc.LigoCampaignGateway.Domain.Entities;
using Jrc.LigoCampaignGateway.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Jrc.LigoCampaignGateway.Application.Services;

public class MediaLeaseService : IMediaLeaseService
{
    private readonly IAppDbContext _db;
    private readonly ILigoMediaClient _ligoMediaClient;
    private readonly IMediaAssetStorage _storage;
    private readonly ILogger<MediaLeaseService> _logger;

    public MediaLeaseService(
        IAppDbContext db,
        ILigoMediaClient ligoMediaClient,
        IMediaAssetStorage storage,
        ILogger<MediaLeaseService> logger)
    {
        _db = db;
        _ligoMediaClient = ligoMediaClient;
        _storage = storage;
        _logger = logger;
    }

    public async Task<PrepareMediaResponse> PrepareMediaAsync(PrepareMediaRequest request, CancellationToken ct = default)
    {
        var template = _db.Templates.FirstOrDefault(t => t.Id == request.TemplateId && t.Active);
        if (template == null)
        {
            return new PrepareMediaResponse(false, string.Empty, string.Empty, null, "Template not found or inactive.");
        }

        var asset = _db.MediaAssets.FirstOrDefault(a => a.Id == request.MediaAssetId && a.Active);
        if (asset == null)
        {
            return new PrepareMediaResponse(false, string.Empty, string.Empty, null, "Media asset not found or inactive.");
        }

        LigoMediaUploadResult uploadResult;
        if (request.Mode.Equals("Url", StringComparison.OrdinalIgnoreCase))
        {
            uploadResult = await _ligoMediaClient.UploadMediaUrlAsync(asset.PublicUrl, ct);
        }
        else
        {
            using var stream = await _storage.ReadAssetAsync(asset.StoragePath, ct);
            uploadResult = await _ligoMediaClient.UploadMediaStreamAsync(stream, asset.OriginalFileName, asset.ContentType, ct);
        }

        if (string.IsNullOrEmpty(uploadResult.IdMedia))
        {
            return new PrepareMediaResponse(false, string.Empty, string.Empty, null, "Ligo upload failed to return idmedia.");
        }

        var existingLeases = _db.MediaLeases.Where(l => l.TemplateId == template.Id && l.Status == MediaLeaseState.Active).ToList();
        foreach (var oldLease in existingLeases)
        {
            oldLease.Status = MediaLeaseState.Expired;
            await _db.UpdateMediaLeaseAsync(oldLease, ct);
        }

        var lease = new ProviderMediaLease
        {
            Id = Guid.NewGuid(),
            TemplateId = template.Id,
            MediaAssetId = asset.Id,
            Provider = "LIGO",
            ProviderMediaId = uploadResult.IdMedia,
            ValidUntilRaw = uploadResult.ValidUntilRaw,
            ValidUntilParsed = uploadResult.ValidUntilParsed,
            ParseSucceeded = uploadResult.ParseSucceeded,
            Status = MediaLeaseState.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _db.AddMediaLeaseAsync(lease, ct);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Media lease created: {IdMedia}, ValidUntil: {ValidUntil}", lease.ProviderMediaId, lease.ValidUntilRaw);

        return new PrepareMediaResponse(true, lease.ProviderMediaId, lease.ValidUntilRaw, lease.ValidUntilParsed, string.Empty);
    }

    public Task<ProviderMediaLease?> GetActiveLeaseForTemplateAsync(Guid templateId, CancellationToken ct = default)
    {
        var lease = _db.MediaLeases.FirstOrDefault(l => l.TemplateId == templateId && l.Status == MediaLeaseState.Active);
        return Task.FromResult(lease);
    }
}
