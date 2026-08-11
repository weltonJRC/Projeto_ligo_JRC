using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Application.Models;
using Jrc.LigoCampaignGateway.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Jrc.LigoCampaignGateway.Api.Controllers;

[ApiController]
[Route("api/v1/templates")]
public class TemplatesController : ControllerBase
{
    private readonly ITemplateRegistryService _templateService;
    private readonly IMediaLeaseService _mediaLeaseService;

    public TemplatesController(ITemplateRegistryService templateService, IMediaLeaseService mediaLeaseService)
    {
        _templateService = templateService;
        _mediaLeaseService = mediaLeaseService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTemplate([FromBody] WhatsAppTemplate template)
    {
        var result = await _templateService.RegisterTemplateAsync(template, HttpContext.RequestAborted);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTemplates()
    {
        var list = await _templateService.GetAllActiveTemplatesAsync(HttpContext.RequestAborted);
        return Ok(list);
    }

    [HttpPost("{templateId}/prepare-media")]
    public async Task<IActionResult> PrepareMedia(Guid templateId, [FromBody] PrepareMediaRequest request)
    {
        var req = request with { TemplateId = templateId };
        var result = await _mediaLeaseService.PrepareMediaAsync(req, HttpContext.RequestAborted);
        return Ok(result);
    }
}
