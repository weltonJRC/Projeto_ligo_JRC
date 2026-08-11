using Jrc.LigoCampaignGateway.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Jrc.LigoCampaignGateway.Infrastructure.LigoClients;

public class LigoTokenProvider : ILigoTokenProvider
{
    private readonly ILigoAuthClient _authClient;
    private readonly IConfiguration _config;
    private readonly ILogger<LigoTokenProvider> _logger;
    private static readonly SemaphoreSlim TokenLock = new(1, 1);
    private string? _cachedToken;
    private DateTime _tokenExpiration = DateTime.MinValue;

    public LigoTokenProvider(ILigoAuthClient authClient, IConfiguration config, ILogger<LigoTokenProvider> logger)
    {
        _authClient = authClient;
        _config = config;
        _logger = logger;
    }

    public async Task<string> GetValidTokenAsync(CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiration > DateTime.UtcNow.AddMinutes(5))
        {
            return _cachedToken;
        }

        await TokenLock.WaitAsync(ct);
        try
        {
            if (!string.IsNullOrEmpty(_cachedToken) && _tokenExpiration > DateTime.UtcNow.AddMinutes(5))
            {
                return _cachedToken;
            }

            var login = _config["Ligo:AuthLogin"] ?? "PENDENTE_CONTRATO";
            var password = _config["Ligo:AuthPassword"] ?? "PENDENTE_CONTRATO";

            _logger.LogInformation("Refreshing Ligo JWT token via ILigoAuthClient (in-memory)...");
            var authResult = await _authClient.LoginAsync(login, password, ct);

            _cachedToken = authResult.Token;
            _tokenExpiration = authResult.Expiration;

            _logger.LogInformation("Token refreshed successfully. Valid until {Expiration}", _tokenExpiration);
            return _cachedToken;
        }
        finally
        {
            TokenLock.Release();
        }
    }

    public void InvalidateToken()
    {
        _cachedToken = null;
        _tokenExpiration = DateTime.MinValue;
        _logger.LogWarning("Ligo Token manually invalidated due to 401 Unauthorized response.");
    }
}
