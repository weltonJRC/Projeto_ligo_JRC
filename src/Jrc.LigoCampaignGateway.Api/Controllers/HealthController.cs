using Microsoft.AspNetCore.Mvc;

namespace Jrc.LigoCampaignGateway.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            status = "HEALTHY",
            service = "Jrc.LigoCampaignGateway",
            framework = ".NET 10 LTS",
            timestamp = DateTime.UtcNow
        });
    }
}
