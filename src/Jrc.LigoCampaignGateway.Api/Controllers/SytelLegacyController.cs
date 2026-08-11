using System.Text;
using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jrc.LigoCampaignGateway.Api.Controllers;

[ApiController]
[Route("softdial/Whats_New_API_whatsapp_ativo")]
public class SytelLegacyController : ControllerBase
{
    private readonly IHsmDispatchService _dispatchService;
    private readonly ITemplateRegistryService _templateService;
    private readonly IConfiguration _config;
    private readonly ILogger<SytelLegacyController> _logger;

    public SytelLegacyController(
        IHsmDispatchService dispatchService,
        ITemplateRegistryService templateService,
        IConfiguration config,
        ILogger<SytelLegacyController> logger)
    {
        _dispatchService = dispatchService;
        _templateService = templateService;
        _config = config;
        _logger = logger;
    }

    [HttpGet("SendWhatsAppOutboundTemplate")]
    public async Task<IActionResult> SendWhatsAppOutboundTemplate(
        [FromQuery] string numberchip,
        [FromQuery] string template,
        [FromQuery] string destination,
        [FromQuery] string field1,
        [FromQuery] string field2,
        [FromQuery] string recordId,
        [FromQuery] string campaignRunId = "2026-08")
    {
        _logger.LogInformation("Legacy Sytel GET received for recordId {RecordId}, campaignRunId {CampaignRunId}, template {Template}", recordId, campaignRunId, template);

        if (string.IsNullOrEmpty(recordId))
        {
            return BadRequest("recordId is required for campaign idempotency.");
        }

        var request = new OutboundHsmRequest(
            Source: "SYTEL_LEGACY",
            Campaign: "WhatappJRC_Ativo",
            CampaignRunId: campaignRunId,
            RecordId: recordId,
            NumberChip: numberchip,
            Destination: destination,
            TemplateId: template,
            BodyParameters: new[] { field1 ?? string.Empty, field2 ?? string.Empty }
        );

        var response = await _dispatchService.DispatchHsmAsync(request, HttpContext.RequestAborted);
        var alwaysHttp200 = _config.GetValue<bool>("Sytel:LegacyAlwaysHttp200", true);

        if (response.Ok)
        {
            var textResult = $"WHATSAPP_ACCEPTED|correlationId={response.CorrelationId}|status={response.Status}";
            return Content(textResult, "text/plain");
        }
        else
        {
            var textResult = $"FAILED|code={response.Status}|error={response.Error}|correlationId={response.CorrelationId}";
            if (alwaysHttp200)
            {
                return Content(textResult, "text/plain");
            }
            return StatusCode(500, textResult);
        }
    }

    [HttpGet("GetWhatsAppOutboundTemplatesCollection")]
    public async Task<IActionResult> GetWhatsAppOutboundTemplatesCollection([FromQuery] string numberchip)
    {
        var templates = await _templateService.GetAllActiveTemplatesAsync(HttpContext.RequestAborted);

        var xmlBuilder = new StringBuilder();
        xmlBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        xmlBuilder.AppendLine("<collection>");
        xmlBuilder.AppendLine("  <entries>");

        foreach (var t in templates)
        {
            xmlBuilder.AppendLine($"    <entry key=\"{t.ProviderTemplateId}\">{System.Security.SecurityElement.Escape(t.Name)}</entry>");
        }

        xmlBuilder.AppendLine("  </entries>");
        xmlBuilder.AppendLine("</collection>");

        return Content(xmlBuilder.ToString(), "application/xml");
    }
}
