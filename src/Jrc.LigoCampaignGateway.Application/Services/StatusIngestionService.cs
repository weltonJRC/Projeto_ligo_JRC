using System.Text.Json;
using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Application.Models;
using Jrc.LigoCampaignGateway.Domain.Entities;
using Jrc.LigoCampaignGateway.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jrc.LigoCampaignGateway.Application.Services;

public class StatusIngestionService : IStatusIngestionService
{
    private readonly IAppDbContext _db;
    private readonly ILogger<StatusIngestionService> _logger;

    public StatusIngestionService(IAppDbContext db, ILogger<StatusIngestionService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ProcessStatusWebhookAsync(LigoStatusWebhookPayload payload, CancellationToken ct = default)
    {
        _logger.LogInformation("Webhook status received: MessageId={MessageId}, Status={Status}", payload.MessageId, payload.Status);

        var evt = new ProviderStatusEvent
        {
            Id = Guid.NewGuid(),
            ProviderMessageId = payload.MessageId,
            EventStatus = payload.Status,
            PayloadJson = JsonSerializer.Serialize(payload),
            ReceivedAt = DateTime.UtcNow
        };
        await _db.AddStatusEventAsync(evt, ct);

        // H3: Use FirstOrDefaultAsync instead of synchronous FirstOrDefault
        var dispatch = _db.Dispatches
            .FirstOrDefault(d => d.ProviderMessageId == payload.MessageId || d.ProviderCorrelationId == payload.MessageId);

        if (dispatch != null)
        {
            var targetState = MapStatus(payload.Status);

            if (CanTransition(dispatch.Status, targetState))
            {
                await _db.UpdateDispatchStateAsync(dispatch.Id, targetState, payload.MessageId, payload.ErrorMessage, ct);
                _logger.LogInformation("Dispatch {Id} state updated from {Old} to {New}", dispatch.Id, dispatch.Status, targetState);
            }
            else
            {
                _logger.LogWarning("Out-of-order status ignored for Dispatch {Id}: current state is {Current}, incoming target state was {Target}", dispatch.Id, dispatch.Status, targetState);
            }
        }
        else
        {
            _logger.LogWarning("No dispatch found for webhook MessageId={MessageId}. Event stored but no state update.", payload.MessageId);
        }

        await _db.SaveChangesAsync(ct);
    }

    private static bool CanTransition(DispatchState current, DispatchState target)
    {
        if (current == target) return true;

        // Terminal state Read cannot regress
        if (current == DispatchState.Read) return false;

        // Delivered cannot regress to Sent, Accepted or Sending
        if (current == DispatchState.Delivered && target is DispatchState.Sent or DispatchState.Accepted or DispatchState.Sending) return false;

        // Sent cannot regress to Accepted or Sending
        if (current == DispatchState.Sent && target is DispatchState.Accepted or DispatchState.Sending) return false;

        return true;
    }

    private static DispatchState MapStatus(string status)
    {
        return status.ToUpperInvariant() switch
        {
            "ACCEPTED" => DispatchState.Accepted,
            "SENT" => DispatchState.Sent,
            "DELIVERED" => DispatchState.Delivered,
            "READ" => DispatchState.Read,
            "FAILED" or "REJECTED" => DispatchState.FailedPermanent,
            _ => DispatchState.Unknown
        };
    }
}
