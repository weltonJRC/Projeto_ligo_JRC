using Jrc.LigoCampaignGateway.Domain.Enums;

namespace Jrc.LigoCampaignGateway.Domain.Entities;

public class ProviderMediaLease
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TemplateId { get; set; }
    public Guid MediaAssetId { get; set; }
    public string Provider { get; set; } = "LIGO";
    public string ProviderMediaId { get; set; } = string.Empty;
    public string ValidUntilRaw { get; set; } = string.Empty;
    public DateTime? ValidUntilParsed { get; set; }
    public bool ParseSucceeded { get; set; }
    public MediaLeaseState Status { get; set; } = MediaLeaseState.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
