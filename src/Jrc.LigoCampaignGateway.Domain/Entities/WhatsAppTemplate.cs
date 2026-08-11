namespace Jrc.LigoCampaignGateway.Domain.Entities;

public class WhatsAppTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Tenant { get; set; } = "grupojrc";
    public string NumberChip { get; set; } = "551148004100";
    public string ProviderTemplateId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Language { get; set; } = "pt_BR";
    public string Category { get; set; } = "MARKETING";
    public string HeaderType { get; set; } = "IMAGE";
    public int ParameterCount { get; set; } = 2;
    public string CallbackStatusUrl { get; set; } = string.Empty;
    public string CallbackResponsesUrl { get; set; } = string.Empty;
    public string Status { get; set; } = "READY";
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
