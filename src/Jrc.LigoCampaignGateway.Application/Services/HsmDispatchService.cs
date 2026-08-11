using System.Security.Cryptography;
using System.Text;
using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Application.Models;
using Jrc.LigoCampaignGateway.Domain.Entities;
using Jrc.LigoCampaignGateway.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Jrc.LigoCampaignGateway.Application.Services;

public class HsmDispatchService : IHsmDispatchService
{
    private readonly IAppDbContext _db;
    private readonly ITemplateRegistryService _templateService;
    private readonly IMediaLeaseService _mediaLeaseService;
    private readonly ILigoHsmClient _hsmClient;
    private readonly ILogger<HsmDispatchService> _logger;

    public HsmDispatchService(
        IAppDbContext db,
        ITemplateRegistryService templateService,
        IMediaLeaseService mediaLeaseService,
        ILigoHsmClient hsmClient,
        ILogger<HsmDispatchService> logger)
    {
        _db = db;
        _templateService = templateService;
        _mediaLeaseService = mediaLeaseService;
        _hsmClient = hsmClient;
        _logger = logger;
    }

    public async Task<OutboundHsmResponse> DispatchHsmAsync(OutboundHsmRequest request, CancellationToken ct = default)
    {
        var tenant = "grupojrc";
        var allowedNumberChip = "551148004100";

        if (request.NumberChip != allowedNumberChip)
        {
            _logger.LogWarning("Unauthorized NumberChip: {NumberChip}", request.NumberChip);
            return new OutboundHsmResponse(false, string.Empty, "FAILED", null, $"NumberChip '{request.NumberChip}' not authorized.", null);
        }

        var campaignRunId = string.IsNullOrWhiteSpace(request.CampaignRunId) ? "2026-08" : request.CampaignRunId;
        var correlationId = $"{tenant}:{request.Campaign}:{campaignRunId}:{request.RecordId}:{request.TemplateId}";

        // Check for Idempotency
        var existing = _db.Dispatches.FirstOrDefault(d =>
            d.Tenant == tenant &&
            d.Campaign == request.Campaign &&
            d.CampaignRunId == campaignRunId &&
            d.RecordId == request.RecordId &&
            d.TemplateId.ToString() == request.TemplateId ||
            d.ProviderCorrelationId == correlationId);

        if (existing != null)
        {
            _logger.LogInformation("Idempotency match found for correlation {CorrelationId}. Current status: {Status}", correlationId, existing.Status);
            if (existing.Status is DispatchState.Accepted or DispatchState.Sent or DispatchState.Delivered or DispatchState.Read)
            {
                return new OutboundHsmResponse(true, correlationId, "ALREADY_ACCEPTED", existing.ProviderMessageId, null, null);
            }
            if (existing.Status is DispatchState.Sending or DispatchState.Reserved)
            {
                return new OutboundHsmResponse(true, correlationId, "PROCESSING", existing.ProviderMessageId, null, null);
            }
        }

        var template = await _templateService.GetTemplateByProviderIdAsync(request.TemplateId, ct);
        if (template == null || !template.Active)
        {
            return new OutboundHsmResponse(false, correlationId, "FAILED", null, $"Template '{request.TemplateId}' not found or inactive.", null);
        }

        var lease = await _mediaLeaseService.GetActiveLeaseForTemplateAsync(template.Id, ct);
        if (lease == null || lease.Status != MediaLeaseState.Active)
        {
            return new OutboundHsmResponse(false, correlationId, "FAILED", null, $"No active media lease found for template '{request.TemplateId}'.", null);
        }

        // PII Masking: SHA-256 Hash of destination phone
        var destHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.Destination)));
        var destLast4 = request.Destination.Length >= 4 ? request.Destination[^4..] : request.Destination;

        var dispatch = new MessageDispatch
        {
            Id = Guid.NewGuid(),
            Tenant = tenant,
            Campaign = request.Campaign,
            CampaignRunId = campaignRunId,
            RecordId = request.RecordId,
            ProviderCorrelationId = correlationId,
            TemplateId = template.Id,
            MediaLeaseId = lease.Id,
            NumberChip = request.NumberChip,
            DestinationHash = destHash,
            DestinationLast4 = destLast4,
            Status = DispatchState.Sending,
            CreatedAt = DateTime.UtcNow
        };

        await _db.AddDispatchAsync(dispatch, ct);
        await _db.SaveChangesAsync(ct);

        var ligoMessage = new LigoHsmMessage(
            CorrelationId: correlationId,
            NumberChip: request.NumberChip,
            Telephone: request.Destination,
            Template: template.ProviderTemplateId,
            IdMedia: lease.ProviderMediaId,
            BodyParameters: request.BodyParameters,
            StatusCallbackUrl: template.CallbackStatusUrl,
            ResponseCallbackUrl: template.CallbackResponsesUrl
        );

        LigoSendResult sendResult;
        try
        {
            sendResult = await _hsmClient.SendTemplateWithMediaAsync(new[] { ligoMessage }, ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogError("Timeout occurred while sending HSM for correlation {CorrelationId}. Setting state to Unknown.", correlationId);
            await _db.UpdateDispatchStateAsync(dispatch.Id, DispatchState.Unknown, null, "Ambiguous HTTP timeout during send.", ct);
            await _db.SaveChangesAsync(ct);

            return new OutboundHsmResponse(false, correlationId, "UNKNOWN", null, "HTTP timeout during dispatch; state marked Unknown.", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HTTP exception during HSM dispatch for correlation {CorrelationId}", correlationId);
            await _db.UpdateDispatchStateAsync(dispatch.Id, DispatchState.Unknown, null, ex.Message, ct);
            await _db.SaveChangesAsync(ct);

            return new OutboundHsmResponse(false, correlationId, "UNKNOWN", null, $"Transport exception: {ex.Message}", null);
        }

        if (sendResult.Success && sendResult.Status == "ACCEPTED")
        {
            await _db.UpdateDispatchStateAsync(dispatch.Id, DispatchState.Accepted, sendResult.ProviderMessageId, null, ct);
            await _db.SaveChangesAsync(ct);

            var mediaInfo = new MediaLeaseInfoDto(lease.ProviderMediaId, lease.ValidUntilRaw);
            return new OutboundHsmResponse(true, correlationId, "ACCEPTED", sendResult.ProviderMessageId, null, mediaInfo);
        }
        else
        {
            await _db.UpdateDispatchStateAsync(dispatch.Id, DispatchState.FailedPermanent, null, sendResult.ErrorMessage, ct);
            await _db.SaveChangesAsync(ct);

            return new OutboundHsmResponse(false, correlationId, "FAILED", null, sendResult.ErrorMessage, null);
        }
    }
}
