using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Application.Models;
using Jrc.LigoCampaignGateway.Application.Services;
using Jrc.LigoCampaignGateway.Domain.Entities;
using Jrc.LigoCampaignGateway.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Jrc.LigoCampaignGateway.UnitTests;

public class HsmDispatchServiceTests
{
    private readonly Mock<IAppDbContext> _dbMock;
    private readonly Mock<ITemplateRegistryService> _templateServiceMock;
    private readonly Mock<IMediaLeaseService> _mediaLeaseServiceMock;
    private readonly Mock<ILigoHsmClient> _hsmClientMock;
    private readonly HsmDispatchService _service;

    public HsmDispatchServiceTests()
    {
        _dbMock = new Mock<IAppDbContext>();
        _templateServiceMock = new Mock<ITemplateRegistryService>();
        _mediaLeaseServiceMock = new Mock<IMediaLeaseService>();
        _hsmClientMock = new Mock<ILigoHsmClient>();

        var dispatches = new List<MessageDispatch>().AsQueryable();
        _dbMock.Setup(d => d.Dispatches).Returns(dispatches);

        _service = new HsmDispatchService(
            _dbMock.Object,
            _templateServiceMock.Object,
            _mediaLeaseServiceMock.Object,
            _hsmClientMock.Object,
            NullLogger<HsmDispatchService>.Instance
        );
    }

    [Fact]
    public async Task DispatchHsm_UnauthorizedNumberChip_ReturnsFailedResponse()
    {
        var req = new OutboundHsmRequest("SYTEL", "WhatappJRC_Ativo", "2026-08", "101", "551100000000", "5511999999999", "template123", new[] { "A" });

        var response = await _service.DispatchHsmAsync(req);

        Assert.False(response.Ok);
        Assert.Equal("FAILED", response.Status);
        Assert.Contains("not authorized", response.Error);
    }

    [Fact]
    public async Task DispatchHsm_TemplateNotFound_ReturnsFailedResponse()
    {
        var req = new OutboundHsmRequest("SYTEL", "WhatappJRC_Ativo", "2026-08", "101", "551148004100", "5511999999999", "invalid_template", new[] { "A" });
        _templateServiceMock.Setup(t => t.GetTemplateByProviderIdAsync("invalid_template", It.IsAny<CancellationToken>()))
            .ReturnsAsync((WhatsAppTemplate?)null);

        var response = await _service.DispatchHsmAsync(req);

        Assert.False(response.Ok);
        Assert.Contains("not found", response.Error);
    }

    [Fact]
    public async Task DispatchHsm_ValidRequest_ExecutesAndReturnsAccepted()
    {
        var templateId = "valid_template";
        var req = new OutboundHsmRequest("SYTEL", "WhatappJRC_Ativo", "2026-08", "101", "551148004100", "5511999999999", templateId, new[] { "João", "31/08" });

        var template = new WhatsAppTemplate { ProviderTemplateId = templateId, Active = true };
        var lease = new ProviderMediaLease { ProviderMediaId = "media123", ValidUntilRaw = "31/12/2099", Status = MediaLeaseState.Active };

        _templateServiceMock.Setup(t => t.GetTemplateByProviderIdAsync(templateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);

        _mediaLeaseServiceMock.Setup(m => m.GetActiveLeaseForTemplateAsync(template.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(lease);

        _hsmClientMock.Setup(h => h.SendTemplateWithMediaAsync(It.IsAny<IReadOnlyList<LigoHsmMessage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LigoSendResult(true, "ACCEPTED", "wamid.123", null, null, "{}"));

        var response = await _service.DispatchHsmAsync(req);

        Assert.True(response.Ok);
        Assert.Equal("ACCEPTED", response.Status);
        Assert.Equal("wamid.123", response.ProviderMessageId);
        Assert.Equal("media123", response.Media?.IdMedia);
    }
}
