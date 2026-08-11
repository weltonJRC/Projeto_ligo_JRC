using System.Text;
using System.Text.Json;
using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Jrc.LigoCampaignGateway.Infrastructure.LigoClients;

public class LigoHsmClient : ILigoHsmClient
{
    private readonly HttpClient _httpClient;
    private readonly ILigoTokenProvider _tokenProvider;
    private readonly IConfiguration _config;
    private readonly ILogger<LigoHsmClient> _logger;

    public LigoHsmClient(HttpClient httpClient, ILigoTokenProvider tokenProvider, IConfiguration config, ILogger<LigoHsmClient> logger)
    {
        _httpClient = httpClient;
        _tokenProvider = tokenProvider;
        _config = config;
        _logger = logger;
    }

    public async Task<LigoSendResult> SendTemplateWithMediaAsync(IReadOnlyList<LigoHsmMessage> messages, CancellationToken ct = default)
    {
        var token = await _tokenProvider.GetValidTokenAsync(ct);
        var baseUrl = _config["Ligo:MessagingBaseUrl"] ?? "https://apiwhatsapp.messaging.digitalcontact.cloud";
        var path = _config["Ligo:HsmSendPath"] ?? "/v1/message/send";

        var payloadList = messages.Select(m => {
            var dict = new Dictionary<string, object>
            {
                ["id"] = m.CorrelationId,
                ["numberchip"] = m.NumberChip,
                ["telephone"] = m.Telephone,
                ["template"] = m.Template,
                ["idmedia"] = m.IdMedia
            };

            for (int i = 0; i < m.BodyParameters.Count; i++)
            {
                dict[$"field{(i + 1):D2}"] = m.BodyParameters[i];
            }

            if (!string.IsNullOrEmpty(m.StatusCallbackUrl)) dict["callbackStatus"] = m.StatusCallbackUrl;
            if (!string.IsNullOrEmpty(m.ResponseCallbackUrl)) dict["callbackResponses"] = m.ResponseCallbackUrl;

            return dict;
        }).ToList();

        var json = JsonSerializer.Serialize(payloadList);
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}{path}");
        request.Headers.Add("x-access-token", token);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request, ct);
        var responseText = await response.Content.ReadAsStringAsync(ct);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Received 401 Unauthorized from Ligo HSM Send. Invalidation & single retry.");
            _tokenProvider.InvalidateToken();
            token = await _tokenProvider.GetValidTokenAsync(ct);

            using var retryReq = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}{path}");
            retryReq.Headers.Add("x-access-token", token);
            retryReq.Content = new StringContent(json, Encoding.UTF8, "application/json");

            response = await _httpClient.SendAsync(retryReq, ct);
            responseText = await response.Content.ReadAsStringAsync(ct);
        }

        if (!response.IsSuccessStatusCode)
        {
            return new LigoSendResult(false, "FAILED", null, ((int)response.StatusCode).ToString(), responseText, responseText);
        }

        return new LigoSendResult(true, "ACCEPTED", $"wamid_{Guid.NewGuid():N}", null, null, responseText);
    }
}
