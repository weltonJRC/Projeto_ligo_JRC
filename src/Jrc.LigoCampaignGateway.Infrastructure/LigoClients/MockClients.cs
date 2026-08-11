using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Application.Models;
using Microsoft.Extensions.Logging;

namespace Jrc.LigoCampaignGateway.Infrastructure.LigoClients;

public class MockLigoAuthClient : ILigoAuthClient
{
    public Task<LigoAuthResult> LoginAsync(string login, string password, CancellationToken ct = default)
    {
        var mockResult = new LigoAuthResult($"mock_jwt_token_{Guid.NewGuid():N}", DateTime.UtcNow.AddHours(23), "{\"token\":\"mock\"}");
        return Task.FromResult(mockResult);
    }
}

public class MockLigoMediaClient : ILigoMediaClient
{
    private readonly ILogger<MockLigoMediaClient> _logger;

    public MockLigoMediaClient(ILogger<MockLigoMediaClient> logger)
    {
        _logger = logger;
    }

    public Task<LigoMediaUploadResult> UploadMediaStreamAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        _logger.LogInformation("[MOCK MEDIA] Uploading stream {FileName} ({Length} bytes)", fileName, stream.Length);
        var mockIdMedia = $"mock_media_{Guid.NewGuid():N}";
        var result = new LigoMediaUploadResult(mockIdMedia, "31/12/2099", new DateTime(2099, 12, 31, 23, 59, 59, DateTimeKind.Utc), true, $"{{\"idmedia\":\"{mockIdMedia}\",\"validUntil\":\"31/12/2099\"}}");
        return Task.FromResult(result);
    }

    public Task<LigoMediaUploadResult> UploadMediaUrlAsync(string publicUrl, CancellationToken ct = default)
    {
        _logger.LogInformation("[MOCK MEDIA] Uploading URL {Url}", publicUrl);
        var mockIdMedia = $"mock_media_url_{Guid.NewGuid():N}";
        var result = new LigoMediaUploadResult(mockIdMedia, "31/12/2099", new DateTime(2099, 12, 31, 23, 59, 59, DateTimeKind.Utc), true, $"{{\"idmedia\":\"{mockIdMedia}\",\"validUntil\":\"31/12/2099\"}}");
        return Task.FromResult(result);
    }
}

public class MockLigoHsmClient : ILigoHsmClient
{
    private readonly ILogger<MockLigoHsmClient> _logger;

    public MockLigoHsmClient(ILogger<MockLigoHsmClient> logger)
    {
        _logger = logger;
    }

    public Task<LigoSendResult> SendTemplateWithMediaAsync(IReadOnlyList<LigoHsmMessage> messages, CancellationToken ct = default)
    {
        var msg = messages.FirstOrDefault();
        _logger.LogInformation("[MOCK HSM] Dispatching template {Template} to {Telephone} with correlation {Correlation}", msg?.Template, msg?.Telephone, msg?.CorrelationId);

        var mockWamid = $"wamid.MOCK_{Guid.NewGuid():N}";
        var result = new LigoSendResult(true, "ACCEPTED", mockWamid, null, null, $"[{{\"status\":\"ACCEPTED\",\"id\":\"{mockWamid}\"}}]");
        return Task.FromResult(result);
    }
}
