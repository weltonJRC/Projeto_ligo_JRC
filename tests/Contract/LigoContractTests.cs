using System.Net;
using Jrc.LigoCampaignGateway.Infrastructure.LigoClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace Jrc.LigoCampaignGateway.ContractTests;

public class LigoContractTests : IDisposable
{
    private readonly WireMockServer _server;

    public LigoContractTests()
    {
        _server = WireMockServer.Start();
    }

    [Fact]
    public async Task LigoAuthClient_LoginAsync_ReturnsValidTokenOn200()
    {
        _server.Given(
            Request.Create().WithPath("/auth/login").UsingPost()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"token\": \"mock_wiremock_jwt_token_12345\"}")
        );

        var myConfig = new Dictionary<string, string?>
        {
            ["Ligo:AuthBaseUrl"] = _server.Url,
            ["Ligo:AuthPath"] = "/auth/login"
        };
        var config = new ConfigurationBuilder().AddInMemoryCollection(myConfig!).Build();
        var client = new LigoAuthClient(new HttpClient(), config, NullLogger<LigoAuthClient>.Instance);

        var result = await client.LoginAsync("user", "pass");

        Assert.Equal("mock_wiremock_jwt_token_12345", result.Token);
    }

    [Fact]
    public async Task LigoAuthClient_LoginAsync_ThrowsOnHttpError()
    {
        _server.Given(
            Request.Create().WithPath("/auth/login").UsingPost()
        ).RespondWith(
            Response.Create().WithStatusCode(401)
        );

        var myConfig = new Dictionary<string, string?>
        {
            ["Ligo:AuthBaseUrl"] = _server.Url,
            ["Ligo:AuthPath"] = "/auth/login"
        };
        var config = new ConfigurationBuilder().AddInMemoryCollection(myConfig!).Build();
        var client = new LigoAuthClient(new HttpClient(), config, NullLogger<LigoAuthClient>.Instance);

        await Assert.ThrowsAsync<InvalidOperationException>(() => client.LoginAsync("user", "wrong"));
    }

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }
}
