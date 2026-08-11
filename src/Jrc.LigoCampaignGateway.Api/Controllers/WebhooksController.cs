using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jrc.LigoCampaignGateway.Api.Controllers;

[ApiController]
[Route("api/v1/webhooks/ligo")]
public class WebhooksController : ControllerBase
{
    private readonly IStatusIngestionService _statusService;

    public WebhooksController(IStatusIngestionService statusService)
    {
        _statusService = statusService;
    }

    [HttpPost("status")]
    public async Task<IActionResult> ReceiveStatusWebhook([FromBody] LigoStatusWebhookPayload payload)
    {
        await _statusService.ProcessStatusWebhookAsync(payload, HttpContext.RequestAborted);
        return Ok(new { received = true });
    }
}
