using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jrc.LigoCampaignGateway.Api.Controllers;

[ApiController]
[Route("api/v1/outbound")]
public class OutboundHsmController : ControllerBase
{
    private readonly IHsmDispatchService _dispatchService;

    public OutboundHsmController(IHsmDispatchService dispatchService)
    {
        _dispatchService = dispatchService;
    }

    [HttpPost("hsm-media")]
    public async Task<IActionResult> PostOutboundHsm([FromBody] OutboundHsmRequest request)
    {
        var result = await _dispatchService.DispatchHsmAsync(request, HttpContext.RequestAborted);
        if (result.Ok) return Ok(result);
        return BadRequest(result);
    }
}
