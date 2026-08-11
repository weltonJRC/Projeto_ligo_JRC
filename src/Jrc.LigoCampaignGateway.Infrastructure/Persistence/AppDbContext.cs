using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Domain.Entities;
using Jrc.LigoCampaignGateway.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Jrc.LigoCampaignGateway.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<WhatsAppTemplate> TemplatesSet => Set<WhatsAppTemplate>();
    public DbSet<MediaAsset> MediaAssetsSet => Set<MediaAsset>();
    public DbSet<ProviderMediaLease> MediaLeasesSet => Set<ProviderMediaLease>();
    public DbSet<MessageDispatch> DispatchesSet => Set<MessageDispatch>();
    public DbSet<ProviderStatusEvent> StatusEventsSet => Set<ProviderStatusEvent>();

    public IQueryable<WhatsAppTemplate> Templates => TemplatesSet;
    public IQueryable<MediaAsset> MediaAssets => MediaAssetsSet;
    public IQueryable<ProviderMediaLease> MediaLeases => MediaLeasesSet;
    public IQueryable<MessageDispatch> Dispatches => DispatchesSet;
    public IQueryable<ProviderStatusEvent> StatusEvents => StatusEventsSet;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraint for Idempotency: (Tenant, Campaign, CampaignRunId, RecordId, TemplateId)
        modelBuilder.Entity<MessageDispatch>()
            .HasIndex(d => new { d.Tenant, d.Campaign, d.CampaignRunId, d.RecordId, d.TemplateId })
            .IsUnique();

        modelBuilder.Entity<MessageDispatch>()
            .HasIndex(d => d.ProviderCorrelationId)
            .IsUnique();

        modelBuilder.Entity<WhatsAppTemplate>()
            .HasIndex(t => t.ProviderTemplateId)
            .IsUnique();

        modelBuilder.Entity<MediaAsset>()
            .HasIndex(a => a.Sha256);
    }

    public async Task AddTemplateAsync(WhatsAppTemplate template, CancellationToken ct = default)
    {
        await TemplatesSet.AddAsync(template, ct);
    }

    public async Task AddMediaAssetAsync(MediaAsset asset, CancellationToken ct = default)
    {
        await MediaAssetsSet.AddAsync(asset, ct);
    }

    public async Task AddMediaLeaseAsync(ProviderMediaLease lease, CancellationToken ct = default)
    {
        await MediaLeasesSet.AddAsync(lease, ct);
    }

    public async Task AddDispatchAsync(MessageDispatch dispatch, CancellationToken ct = default)
    {
        await DispatchesSet.AddAsync(dispatch, ct);
    }

    public async Task AddStatusEventAsync(ProviderStatusEvent statusEvent, CancellationToken ct = default)
    {
        await StatusEventsSet.AddAsync(statusEvent, ct);
    }

    public Task UpdateDispatchStateAsync(Guid dispatchId, DispatchState state, string? providerMessageId = null, string? error = null, CancellationToken ct = default)
    {
        var dispatch = DispatchesSet.FirstOrDefault(d => d.Id == dispatchId);
        if (dispatch != null)
        {
            dispatch.Status = state;
            if (state == DispatchState.Accepted) dispatch.AcceptedAt = DateTime.UtcNow;
            if (state is DispatchState.Delivered or DispatchState.Read or DispatchState.FailedPermanent) dispatch.CompletedAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(providerMessageId)) dispatch.ProviderMessageId = providerMessageId;
            if (!string.IsNullOrEmpty(error)) dispatch.LastErrorMessage = error;
        }
        return Task.CompletedTask;
    }

    public Task UpdateMediaLeaseAsync(ProviderMediaLease lease, CancellationToken ct = default)
    {
        MediaLeasesSet.Update(lease);
        return Task.CompletedTask;
    }
}
