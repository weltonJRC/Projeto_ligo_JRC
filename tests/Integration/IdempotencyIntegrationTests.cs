using Jrc.LigoCampaignGateway.Domain.Entities;
using Jrc.LigoCampaignGateway.Domain.Enums;
using Jrc.LigoCampaignGateway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Jrc.LigoCampaignGateway.IntegrationTests;

public class IdempotencyIntegrationTests
{
    private AppDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AppDbContext_StoresAndRetrievesDispatch_WithIdempotencyKeys()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        var dispatch = new MessageDispatch
        {
            Id = Guid.NewGuid(),
            Tenant = "grupojrc",
            Campaign = "WhatappJRC_Ativo",
            CampaignRunId = "2026-08",
            RecordId = "1001",
            ProviderCorrelationId = "grupojrc:WhatappJRC_Ativo:2026-08:1001:template1",
            TemplateId = Guid.NewGuid(),
            NumberChip = "551148004100",
            DestinationHash = "hash123",
            DestinationLast4 = "9999",
            Status = DispatchState.Accepted,
            CreatedAt = DateTime.UtcNow
        };

        await context.AddDispatchAsync(dispatch);
        await context.SaveChangesAsync();

        var retrieved = await context.Dispatches.FirstOrDefaultAsync(d =>
            d.Tenant == "grupojrc" &&
            d.Campaign == "WhatappJRC_Ativo" &&
            d.CampaignRunId == "2026-08" &&
            d.RecordId == "1001");

        Assert.NotNull(retrieved);
        Assert.Equal(DispatchState.Accepted, retrieved.Status);
        Assert.Equal("grupojrc:WhatappJRC_Ativo:2026-08:1001:template1", retrieved.ProviderCorrelationId);
    }
}
