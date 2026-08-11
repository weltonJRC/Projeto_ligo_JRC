using System.Text;
using System.Text.Json;
using Jrc.LigoCampaignGateway.Application.Abstractions;
using Jrc.LigoCampaignGateway.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Jrc.LigoCampaignGateway.Infrastructure.LigoClients;

public class LigoAuthClient : ILigoAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<LigoAuthClient> _logger;

    public LigoAuthClient(HttpClient httpClient, IConfiguration config, ILogger<LigoAuthClient> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<LigoAuthResult> LoginAsync(string login, string password, CancellationToken ct = default)
    {
        var baseUrl = _config["Ligo:AuthBaseUrl"] ?? "https://api.messaging.digitalcontact.cloud";
        var path = _config["Ligo:AuthPath"] ?? "/auth/login";

        var payload = new { login, password };
        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{baseUrl.TrimEnd('/')}{path}", content, ct);
        var responseText = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Ligo Auth HTTP {Code}: {Response}", (int)response.StatusCode, responseText);
            throw new InvalidOperationException($"Ligo Auth login failed: {response.StatusCode}");
        }

        using var doc = JsonDocument.Parse(responseText);
        var root = doc.RootElement;
        var token = root.GetProperty("token").GetString()
            ?? throw new InvalidOperationException("Property 'token' is missing in auth response.");

        var exp = DateTime.UtcNow.AddHours(23);
        return new LigoAuthResult(token, exp, responseText);
    }
}
