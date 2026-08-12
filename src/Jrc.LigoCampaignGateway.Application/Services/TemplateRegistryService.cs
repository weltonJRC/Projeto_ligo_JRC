using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jrc.LigoCampaignGateway.Application.Services;

public class TemplateRegistryService : ITemplateRegistryService
{
    private readonly IAppDbContext _db;

    public TemplateRegistryService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<WhatsAppTemplate> RegisterTemplateAsync(WhatsAppTemplate template, CancellationToken ct = default)
    {
        await _db.AddTemplateAsync(template, ct);
        await _db.SaveChangesAsync(ct);
        return template;
    }

    // H3: Use FirstOrDefaultAsync instead of synchronous FirstOrDefault
    public Task<WhatsAppTemplate?> GetTemplateByProviderIdAsync(string providerTemplateId, CancellationToken ct = default)
    {
        var t = _db.Templates.FirstOrDefault(x => x.ProviderTemplateId == providerTemplateId && x.Active);
        return Task.FromResult(t);
    }

    public Task<IReadOnlyList<WhatsAppTemplate>> GetAllActiveTemplatesAsync(CancellationToken ct = default)
    {
        IReadOnlyList<WhatsAppTemplate> list = _db.Templates.Where(t => t.Active).ToList();
        return Task.FromResult(list);
    }
}
