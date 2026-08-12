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

            // H2: Null check for BodyParameters before iterating
            if (m.BodyParameters is { Count: > 0 })
            {
                for (int i = 0; i < m.BodyParameters.Count; i++)
                {
                    dict[$"field{(i + 1):D2}"] = m.BodyParameters[i] ?? string.Empty;
                }
            }

            if (!string.IsNullOrEmpty(m.StatusCallbackUrl)) dict["callbackStatus"] = m.StatusCallbackUrl;
            if (!string.IsNullOrEmpty(m.ResponseCallbackUrl)) dict["callbackResponses"] = m.ResponseCallbackUrl;

            return dict;
        }).ToList();

        var json = JsonSerializer.Serialize(payloadList);
        _logger.LogDebug("HSM Send payload: {Payload}", json);

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
            _logger.LogError("Ligo HSM Send failed with HTTP {StatusCode}: {Response}", (int)response.StatusCode, responseText);
            return new LigoSendResult(false, "FAILED", null, ((int)response.StatusCode).ToString(), responseText, responseText);
        }

        // C5: Parse the real provider message ID from Ligo's response instead of generating a fake GUID.
        // Ligo response format: {"data":[{"id":"wamid.xxxxx","status":"ACCEPTED"}]} or similar
        var providerMessageId = ParseProviderMessageId(responseText);
        _logger.LogInformation("HSM Send success. ProviderMessageId={ProviderMessageId}", providerMessageId);

        return new LigoSendResult(true, "ACCEPTED", providerMessageId, null, null, responseText);
    }

    /// <summary>
    /// Attempts to extract the real provider message ID from Ligo's JSON response.
    /// Falls back to correlation-based ID if parsing fails (graceful degradation).
    /// </summary>
    private string ParseProviderMessageId(string responseJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            // Try: {"data":[{"id":"wamid.xxx"}]}
            if (root.TryGetProperty("data", out var dataArr) && dataArr.ValueKind == JsonValueKind.Array && dataArr.GetArrayLength() > 0)
            {
                var first = dataArr[0];
                if (first.TryGetProperty("id", out var idProp))
                    return idProp.GetString() ?? $"parsed_{Guid.NewGuid():N}";
                if (first.TryGetProperty("wamid", out var wamidProp))
                    return wamidProp.GetString() ?? $"parsed_{Guid.NewGuid():N}";
            }

            // Try: {"id":"wamid.xxx"}
            if (root.TryGetProperty("id", out var rootId))
                return rootId.GetString() ?? $"parsed_{Guid.NewGuid():N}";

            // Try: {"wamid":"xxx"}
            if (root.TryGetProperty("wamid", out var rootWamid))
                return rootWamid.GetString() ?? $"parsed_{Guid.NewGuid():N}";

            // Try: array root [{"id":"wamid.xxx"}]
            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
            {
                var first = root[0];
                if (first.TryGetProperty("id", out var arrId))
                    return arrId.GetString() ?? $"parsed_{Guid.NewGuid():N}";
            }
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse Ligo response JSON for provider message ID. Raw: {Raw}", responseJson);
        }

        // Fallback: generate a traceable ID that indicates parsing failed
        _logger.LogWarning("Could not extract provider message ID from Ligo response. Using fallback. Raw: {Raw}", responseJson);
        return $"unparsed_{Guid.NewGuid():N}";
    }
}
