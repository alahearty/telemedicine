using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using NUnit.Framework;

namespace telemedicine_webapi.Application.IntegrationTests.Security;

public class MiddlewareSecurityTests
{
    [Test]
    public async Task Live_health_is_independent_and_sanitized()
    {
        using var client = Testing.CreateClient();
        var response = await client.GetAsync("/health/live");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("self").And.NotContain("exception").And.NotContain("connectionString");
    }

    [Test]
    public async Task Protected_endpoint_rejects_missing_and_malformed_tokens_without_caching()
    {
        using var client = Testing.CreateClient();
        var missing = await client.GetAsync("/api/Appointment/1");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not-a-jwt");
        var malformed = await client.GetAsync("/api/Appointment/1");

        missing.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        malformed.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        missing.Headers.CacheControl!.NoStore.Should().BeTrue();
    }

    [Test]
    public async Task Cors_accepts_allowlisted_origin_and_rejects_other_origins()
    {
        using var client = Testing.CreateClient();
        using var allowed = new HttpRequestMessage(HttpMethod.Options, "/api/auth/login");
        allowed.Headers.Add("Origin", "http://localhost:8080");
        allowed.Headers.Add("Access-Control-Request-Method", "POST");
        var allowedResponse = await client.SendAsync(allowed);

        using var denied = new HttpRequestMessage(HttpMethod.Options, "/api/auth/login");
        denied.Headers.Add("Origin", "https://attacker.invalid");
        denied.Headers.Add("Access-Control-Request-Method", "POST");
        var deniedResponse = await client.SendAsync(denied);

        allowedResponse.Headers.GetValues("Access-Control-Allow-Origin").Should().Contain("http://localhost:8080");
        deniedResponse.Headers.Contains("Access-Control-Allow-Origin").Should().BeFalse();
    }

    [Test]
    public async Task Authentication_rate_limit_returns_429()
    {
        using var client = Testing.CreateClient();
        HttpResponseMessage? response = null;
        for (var attempt = 0; attempt < 11; attempt++)
            response = await client.PostAsJsonAsync("/api/auth/refresh", new { refreshToken = "invalid" });

        response!.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
    }
}
