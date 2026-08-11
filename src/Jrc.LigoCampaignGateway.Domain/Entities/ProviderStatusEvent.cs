namespace Jrc.LigoCampaignGateway.Domain.Entities;

public class ProviderStatusEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ProviderMessageId { get; set; } = string.Empty;
    public string EventStatus { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}
