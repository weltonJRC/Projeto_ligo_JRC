using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Jrc.LigoCampaignGateway.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        var dummyConnectionString = "Host=localhost;Database=jrc_ligo_gateway;Username=postgres;Password=postgres";
        builder.UseNpgsql(dummyConnectionString);
        return new AppDbContext(builder.Options);
    }
}
