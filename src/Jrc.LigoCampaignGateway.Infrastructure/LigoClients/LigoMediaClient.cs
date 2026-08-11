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

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}{path}");
        request.Headers.Add("x-access-token", token);

        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(streamContent, "file", fileName);
        request.Content = content;

        var response = await _httpClient.SendAsync(request, ct);
        var responseText = await response.Content.ReadAsStringAsync(ct);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Received 401 Unauthorized from Ligo Media Upload. Invalidation & single retry.");
            _tokenProvider.InvalidateToken();
            token = await _tokenProvider.GetValidTokenAsync(ct);

            using var retryReq = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}{path}");
            retryReq.Headers.Add("x-access-token", token);
            retryReq.Content = content;

            response = await _httpClient.SendAsync(retryReq, ct);
            responseText = await response.Content.ReadAsStringAsync(ct);
        }

        return ParseMediaResponse(responseText);
    }

    public async Task<LigoMediaUploadResult> UploadMediaUrlAsync(string publicUrl, CancellationToken ct = default)
    {
        var token = await _tokenProvider.GetValidTokenAsync(ct);
        var baseUrl = _config["Ligo:MediaBaseUrl"] ?? "https://api.messaging.digitalcontact.cloud";
        var path = _config["Ligo:MediaUploadPath"] ?? "/media/upload";

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}{path}");
        request.Headers.Add("x-access-token", token);

        var json = JsonSerializer.Serialize(new { file = publicUrl });
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
        using var doc = JsonDocument.Parse(jsonText);
        var root = doc.RootElement;
        var idMedia = root.GetProperty("idmedia").GetString() ?? string.Empty;
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
}
