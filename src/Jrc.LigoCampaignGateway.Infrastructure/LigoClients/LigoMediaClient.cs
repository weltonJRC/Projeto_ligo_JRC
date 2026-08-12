using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Jrc.LigoCampaignGateway.Infrastructure.LigoClients;

public class LigoMediaClient : ILigoMediaClient
{
    private readonly HttpClient _httpClient;
    private readonly ILigoTokenProvider _tokenProvider;
    private readonly IConfiguration _config;
    private readonly ILogger<LigoMediaClient> _logger;

    public LigoMediaClient(HttpClient httpClient, ILigoTokenProvider tokenProvider, IConfiguration config, ILogger<LigoMediaClient> logger)
    {
        _httpClient = httpClient;
        _tokenProvider = tokenProvider;
        _config = config;
        _logger = logger;
    }

    public async Task<LigoMediaUploadResult> UploadMediaStreamAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        var token = await _tokenProvider.GetValidTokenAsync(ct);
        var baseUrl = _config["Ligo:MediaBaseUrl"] ?? "https://api.messaging.digitalcontact.cloud";
        var path = _config["Ligo:MediaUploadPath"] ?? "/media/upload";

        // C6: Buffer the stream into a MemoryStream before sending so we can retry.
        // The original code reused the consumed stream and MultipartFormDataContent on retry,
        // which caused ObjectDisposedException or 0-byte uploads.
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer, ct);

        var responseText = await SendMultipartUploadAsync(baseUrl, path, token, buffer, fileName, contentType, ct);

        // Check if we got a 401 on first try (indicated by the marker)
        if (responseText.StartsWith("__401__"))
        {
            _logger.LogWarning("Received 401 Unauthorized from Ligo Media Upload. Invalidation & single retry.");
            _tokenProvider.InvalidateToken();
            token = await _tokenProvider.GetValidTokenAsync(ct);
            responseText = await SendMultipartUploadAsync(baseUrl, path, token, buffer, fileName, contentType, ct);

            if (responseText.StartsWith("__401__"))
            {
                _logger.LogError("Ligo Media Upload returned 401 even after token refresh.");
                return new LigoMediaUploadResult(string.Empty, string.Empty, null, false, responseText.Replace("__401__", ""));
            }
        }

        return ParseMediaResponse(responseText);
    }

    private async Task<string> SendMultipartUploadAsync(string baseUrl, string path, string token, MemoryStream buffer, string fileName, string contentType, CancellationToken ct)
    {
        // Reset buffer position for each attempt
        buffer.Position = 0;

        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(buffer);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        // Prevent StreamContent from disposing our MemoryStream
        content.Add(streamContent, "file", fileName);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}{path}");
        request.Headers.Add("x-access-token", token);
        request.Content = content;

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, ct);
        var responseText = await response.Content.ReadAsStringAsync(ct);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return $"__401__{responseText}";
        }

        return responseText;
    }

    public async Task<LigoMediaUploadResult> UploadMediaUrlAsync(string publicUrl, CancellationToken ct = default)
    {
        var token = await _tokenProvider.GetValidTokenAsync(ct);
        var baseUrl = _config["Ligo:MediaBaseUrl"] ?? "https://api.messaging.digitalcontact.cloud";
        var path = _config["Ligo:MediaUploadPath"] ?? "/media/upload";

        var json = JsonSerializer.Serialize(new { file = publicUrl });

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}{path}");
        request.Headers.Add("x-access-token", token);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request, ct);
        var responseText = await response.Content.ReadAsStringAsync(ct);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Received 401 Unauthorized from Ligo Media URL Upload. Invalidation & single retry.");
            _tokenProvider.InvalidateToken();
            token = await _tokenProvider.GetValidTokenAsync(ct);

            using var retryReq = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}{path}");
            retryReq.Headers.Add("x-access-token", token);
            retryReq.Content = new StringContent(json, Encoding.UTF8, "application/json");

            response = await _httpClient.SendAsync(retryReq, ct);
            responseText = await response.Content.ReadAsStringAsync(ct);
        }

        return ParseMediaResponse(responseText);
    }

    private static LigoMediaUploadResult ParseMediaResponse(string jsonText)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonText);
            var root = doc.RootElement;

            // Try both "idmedia" and "idMedia" (case variations from provider)
            var idMedia = string.Empty;
            if (root.TryGetProperty("idmedia", out var idLower))
                idMedia = idLower.GetString() ?? string.Empty;
            else if (root.TryGetProperty("idMedia", out var idCamel))
                idMedia = idCamel.GetString() ?? string.Empty;

            var validUntilRaw = root.TryGetProperty("validUntil", out var valProp) ? valProp.GetString() ?? string.Empty : string.Empty;

            DateTime? parsedDate = null;
            var parseOk = false;

            if (!string.IsNullOrEmpty(validUntilRaw))
            {
                if (DateTime.TryParseExact(validUntilRaw, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtExact))
                {
                    parsedDate = DateTime.SpecifyKind(dtExact, DateTimeKind.Utc);
                    parseOk = true;
                }
                else if (DateTime.TryParse(validUntilRaw, out var dtAny))
                {
                    parsedDate = DateTime.SpecifyKind(dtAny, DateTimeKind.Utc);
                    parseOk = true;
                }
            }

            return new LigoMediaUploadResult(idMedia, validUntilRaw, parsedDate, parseOk, jsonText);
        }
        catch (JsonException)
        {
            // Graceful degradation: return empty result instead of crashing
            return new LigoMediaUploadResult(string.Empty, string.Empty, null, false, jsonText);
        }
    }
}
