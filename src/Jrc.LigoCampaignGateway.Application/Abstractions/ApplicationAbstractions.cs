using Jrc.LigoCampaignGateway.Application.Models;
using Jrc.LigoCampaignGateway.Domain.Entities;
using Jrc.LigoCampaignGateway.Domain.Enums;

namespace Jrc.LigoCampaignGateway.Application.Abstractions;

public interface ILigoAuthClient
{
    Task<LigoAuthResult> LoginAsync(string login, string password, CancellationToken ct = default);
}

public interface ILigoTokenProvider
{
    Task<string> GetValidTokenAsync(CancellationToken ct = default);
    void InvalidateToken();
}

public interface ILigoMediaClient
{
    Task<LigoMediaUploadResult> UploadMediaStreamAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
    Task<LigoMediaUploadResult> UploadMediaUrlAsync(string publicUrl, CancellationToken ct = default);
}

public interface ILigoHsmClient
{
    Task<LigoSendResult> SendTemplateWithMediaAsync(IReadOnlyList<LigoHsmMessage> messages, CancellationToken ct = default);
}

public interface IMediaAssetStorage
{
    Task<string> SaveAssetAsync(Stream stream, string originalFileName, CancellationToken ct = default);
    Task<Stream> ReadAssetAsync(string storagePath, CancellationToken ct = default);
}

public interface IAppDbContext
{
    IQueryable<WhatsAppTemplate> Templates { get; }
    IQueryable<MediaAsset> MediaAssets { get; }
    IQueryable<ProviderMediaLease> MediaLeases { get; }
    IQueryable<MessageDispatch> Dispatches { get; }
    IQueryable<ProviderStatusEvent> StatusEvents { get; }

    Task AddTemplateAsync(WhatsAppTemplate template, CancellationToken ct = default);
    Task AddMediaAssetAsync(MediaAsset asset, CancellationToken ct = default);
    Task AddMediaLeaseAsync(ProviderMediaLease lease, CancellationToken ct = default);
    Task AddDispatchAsync(MessageDispatch dispatch, CancellationToken ct = default);
    Task AddStatusEventAsync(ProviderStatusEvent statusEvent, CancellationToken ct = default);
    Task UpdateDispatchStateAsync(Guid dispatchId, DispatchState state, string? providerMessageId = null, string? error = null, CancellationToken ct = default);
    Task UpdateMediaLeaseAsync(ProviderMediaLease lease, CancellationToken ct = default);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public interface ITemplateRegistryService
{
    Task<WhatsAppTemplate> RegisterTemplateAsync(WhatsAppTemplate template, CancellationToken ct = default);
    Task<WhatsAppTemplate?> GetTemplateByProviderIdAsync(string providerTemplateId, CancellationToken ct = default);
    Task<IReadOnlyList<WhatsAppTemplate>> GetAllActiveTemplatesAsync(CancellationToken ct = default);
}

public interface IMediaLeaseService
{
    Task<PrepareMediaResponse> PrepareMediaAsync(PrepareMediaRequest request, CancellationToken ct = default);
    Task<ProviderMediaLease?> GetActiveLeaseForTemplateAsync(Guid templateId, CancellationToken ct = default);
}

public interface IHsmDispatchService
{
    Task<OutboundHsmResponse> DispatchHsmAsync(OutboundHsmRequest request, CancellationToken ct = default);
}

public interface IStatusIngestionService
{
    Task ProcessStatusWebhookAsync(LigoStatusWebhookPayload payload, CancellationToken ct = default);
}
