using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LegalAiAr.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace LegalAiAr.Api.Tests;

public class AuthAndHealthAuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthAndHealthAuthorizationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Health_live_does_not_require_authentication()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/health/live");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Health_full_requires_authentication()
    {
        var client = CreateClientWithoutSession();
        var res = await client.GetAsync("/api/health");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Protected_endpoint_requires_authentication()
    {
        var client = CreateClientWithoutSession();
        var res = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Me_succeeds_with_development_id_token_injection()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await res.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("dev@legal-ai.local", body.GetProperty("email").GetString());
    }

    [Fact]
    public async Task Me_rejects_without_id_token_cookie()
    {
        var client = CreateClientWithoutSession();
        client.DefaultRequestHeaders.Add("X-User-Email", "test@example.com");
        client.DefaultRequestHeaders.Add("X-User-Jwt", JwtTestTokenFactory.Create("test@example.com"));

        var res = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Me_requires_valid_id_token_cookie()
    {
        var client = CreateClientWithSessionJwtValidation();
        var res = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Me_succeeds_with_id_token_cookie()
    {
        var email = "cookie-only@pwc.com";
        var token = JwtTestTokenFactory.Create(email, role: "admin", name: "Cookie User");
        var client = CreateClientWithSessionJwtValidation();
        client.DefaultRequestHeaders.Add("Cookie", $"id_token={token}");

        var res = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await res.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(email, body.GetProperty("email").GetString());
        Assert.Equal("Cookie User", body.GetProperty("displayName").GetString());
    }

    [Fact]
    public async Task Me_maps_role_from_jwt_claim()
    {
        var token = JwtTestTokenFactory.Create("lawyer@pwc.com", role: "lawyer", name: "Maria Lawyer");
        var client = CreateClientWithSessionJwtValidation();
        client.DefaultRequestHeaders.Add("Cookie", $"id_token={token}");

        var res = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await res.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("lawyer", body.GetProperty("role").GetString());
    }

    private HttpClient CreateClientWithSessionJwtValidation()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Auth:Development:InjectIdentity"] = "false",
                    ["Auth:Platform:SigningKeyBase64"] = JwtTestTokenFactory.SigningKeyBase64,
                    ["Auth:Platform:ValidIssuer"] = JwtTestTokenFactory.Issuer,
                    ["Auth:Platform:ValidAudience"] = JwtTestTokenFactory.Audience,
                    ["Auth:Platform:EmailClaim"] = "email",
                });
            });
        });
        return factory.CreateClient();
    }

    private HttpClient CreateClientWithoutSession()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Auth:Development:InjectIdentity"] = "false",
                    ["Auth:Platform:SigningKeyBase64"] = JwtTestTokenFactory.SigningKeyBase64,
                    ["Auth:Platform:ValidIssuer"] = JwtTestTokenFactory.Issuer,
                    ["Auth:Platform:ValidAudience"] = JwtTestTokenFactory.Audience,
                });
            });
        });
        return factory.CreateClient();
    }
}
