using Jrc.LigoCampaignGateway.Domain.Enums;

namespace Jrc.LigoCampaignGateway.Domain.Entities;

public class MessageDispatch
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Tenant { get; set; } = "grupojrc";
    public string Campaign { get; set; } = "WhatappJRC_Ativo";
    public string CampaignRunId { get; set; } = "2026-08";
    public string RecordId { get; set; } = string.Empty;
    public string ProviderCorrelationId { get; set; } = string.Empty;
    public Guid TemplateId { get; set; }
    public Guid? MediaLeaseId { get; set; }
    public string NumberChip { get; set; } = "551148004100";
    public string DestinationHash { get; set; } = string.Empty;
    public string DestinationLast4 { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public DispatchState Status { get; set; } = DispatchState.Reserved;
    public string? ProviderMessageId { get; set; }
    public string? LastErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
