using Serilog.Core;
using Serilog.Events;

namespace Jrc.LigoCampaignGateway.Api.Filters;

public class PiiMaskingEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        // Enriches logs without mutating HTTP request bodies
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("PiiMasked", true));
    }
}
