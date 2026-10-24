using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using telemedicine_webapi.Infrastructure.Persistence;
using telemedicine_webapi.Infrastructure.Persistence.Context;

namespace telemedicine_webapi.Application.IntegrationTests.R1;

internal sealed record AuthSession(string Email, string Password, string Token, string Role, int ProfileId);

internal static class R1TestSupport
{
    public const string AdminEmail = "administrator@localhost";
    public const string AdminPassword = "Administrator1!";
    public const string DefaultPassword = "R1Pass1!x";

    public static async Task SeedBaselineAsync()
    {
        using var scope = Testing.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<IApplicationDbContextInitialiser>();
        await initialiser.SeedAsync();
    }

    public static string UniqueEmail(string prefix) =>
        $"{prefix}.{Guid.NewGuid():N}@r1.test.local";

    public static HttpClient AnonymousClient()
    {
        var client = Testing.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("X-RateLimit-Partition", Guid.NewGuid().ToString("N"));
        return client;
    }

    public static HttpClient AuthorizedClient(string token)
    {
        var client = Testing.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.TryAddWithoutValidation("X-RateLimit-Partition", Guid.NewGuid().ToString("N"));
        return client;
    }

    public static async Task<AuthSession> RegisterAsync(
        HttpClient client,
        string role,
        string? email = null,
        string? license = null)
    {
        var resolvedEmail = email ?? UniqueEmail(role.ToLowerInvariant());
        var payload = new Dictionary<string, object?>
        {
            ["email"] = resolvedEmail,
            ["password"] = DefaultPassword,
            ["accountType"] = role,
            ["firstName"] = "Release",
            ["lastName"] = role,
            ["gender"] = "Female",
            ["birth"] = "1990-04-12",
            ["phone"] = "+27 11 800 1000",
            ["address"] = "1 Integration Street",
            ["emergencyContactName"] = "Integration Contact",
            ["emergencyContactPhone"] = "+27 11 800 1001",
        };
        if (string.Equals(role, "Physician", StringComparison.OrdinalIgnoreCase))
        {
            payload["license"] = license ?? $"R1-{Guid.NewGuid():N}"[..12];
            payload["medicalSpecialization"] = "General Practice";
            payload["qualifications"] = "MBChB (Synthetic)";
            payload["bio"] = "Integration-test physician profile.";
        }

        var response = await client.PostAsJsonAsync("/api/auth/register", payload);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, body);
        var json = JsonDocument.Parse(body).RootElement;
        json.GetProperty("wasSuccesful").GetBoolean().Should().BeTrue(body);
        var data = json.GetProperty("data");
        return new AuthSession(
            resolvedEmail,
            DefaultPassword,
            string.Empty,
            data.GetProperty("role").GetString()!,
            data.GetProperty("id").GetInt32());
    }

    public static async Task<string> LoginAsync(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, body);
        var json = JsonDocument.Parse(body).RootElement;
        return json.GetProperty("data").GetProperty("token").GetString()!;
    }

    public static async Task<AuthSession> RegisterAndLoginAsync(
        string role,
        string? email = null,
        string? license = null)
    {
        using var client = AnonymousClient();
        var session = await RegisterAsync(client, role, email, license);
        var token = await LoginAsync(client, session.Email, session.Password);
        return session with { Token = token };
    }

    public static async Task<string> AdminTokenAsync()
    {
        using var client = AnonymousClient();
        return await LoginAsync(client, AdminEmail, AdminPassword);
    }

    public static async Task<int> CreateServiceAsync(string adminToken)
    {
        using var client = AuthorizedClient(adminToken);
        var name = $"R1 Consultation {Guid.NewGuid():N}"[..30];
        var response = await client.PostAsJsonAsync("/api/TelemedicineServices", new
        {
            serviceName = name,
            cost = 500m,
            description = "Synthetic Release 1 integration-test service."
        });
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, body);
        var json = JsonDocument.Parse(body).RootElement;
        return json.GetProperty("data").GetInt32();
    }

    public static async Task ReplaceDefaultAvailabilityAsync(string physicianToken)
    {
        using var client = AuthorizedClient(physicianToken);
        var rules = new[]
        {
            new { dayOfWeek = 1, startTime = "08:00:00", endTime = "16:00:00", slotDurationMinutes = 30 },
            new { dayOfWeek = 2, startTime = "08:00:00", endTime = "16:00:00", slotDurationMinutes = 30 },
            new { dayOfWeek = 3, startTime = "08:00:00", endTime = "16:00:00", slotDurationMinutes = 30 },
            new { dayOfWeek = 4, startTime = "08:00:00", endTime = "16:00:00", slotDurationMinutes = 30 },
            new { dayOfWeek = 5, startTime = "08:00:00", endTime = "16:00:00", slotDurationMinutes = 30 },
        };
        var response = await client.PutAsJsonAsync("/api/physicians/me/availability", rules);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, body);
    }

    public static async Task<List<JsonElement>> GetSlotsAsync(string token, int physicianId, int minimum = 1)
    {
        using var client = AuthorizedClient(token);
        var from = DateTime.UtcNow.AddDays(1).ToString("O");
        var to = DateTime.UtcNow.AddDays(30).ToString("O");
        var response = await client.GetAsync($"/api/physicians/{physicianId}/slots?from={Uri.EscapeDataString(from)}&to={Uri.EscapeDataString(to)}");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, body);
        var slots = JsonDocument.Parse(body).RootElement.EnumerateArray().Select(x => x.Clone()).ToList();
        slots.Count.Should().BeGreaterThanOrEqualTo(minimum);
        return slots;
    }

    public static async Task<string> CreatePaymentTokenAsync(string patientToken, int serviceId, decimal amount)
    {
        using var client = AuthorizedClient(patientToken);
        var response = await client.PostAsJsonAsync("/api/TelemedicinePayment", new
        {
            telemedicineServiceId = serviceId,
            amountPaid = amount
        });
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, body);
        var json = JsonDocument.Parse(body).RootElement;
        return json.GetProperty("data").GetProperty("paymentToken").GetString()!;
    }

    public static async Task<int> BookAsync(
        string patientToken,
        int serviceId,
        int physicianId,
        JsonElement slot)
    {
        var paymentToken = await CreatePaymentTokenAsync(patientToken, serviceId, 500m);
        return await BookWithTokenAsync(patientToken, serviceId, physicianId, slot, paymentToken);
    }

    public static async Task<int> BookWithTokenAsync(
        string patientToken,
        int serviceId,
        int physicianId,
        JsonElement slot,
        string paymentToken)
    {
        using var client = AuthorizedClient(patientToken);
        var response = await client.PostAsJsonAsync("/api/Appointment", new
        {
            serviceId,
            paymentToken,
            appointmentDate = slot.GetProperty("start").GetDateTime(),
            physicianId,
            durationMinutes = slot.GetProperty("durationMinutes").GetInt32()
        });
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, body);
        var json = JsonDocument.Parse(body).RootElement;
        json.GetProperty("wasSuccesful").GetBoolean().Should().BeTrue(body);
        return json.GetProperty("data").GetProperty("id").GetInt32();
    }

    public static async Task<JsonElement> GetJsonAsync(HttpClient client, string path, HttpStatusCode expected = HttpStatusCode.OK)
    {
        var response = await client.GetAsync(path);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected, body);
        return JsonDocument.Parse(body).RootElement.Clone();
    }

    public static async Task<JsonElement> PostJsonAsync(HttpClient client, string path, object payload, HttpStatusCode expected = HttpStatusCode.OK)
    {
        var response = await client.PostAsJsonAsync(path, payload);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected, body);
        return JsonDocument.Parse(body).RootElement.Clone();
    }

    public static async Task<JsonElement> PutJsonAsync(HttpClient client, string path, object payload, HttpStatusCode expected = HttpStatusCode.OK)
    {
        var response = await client.PutAsJsonAsync(path, payload);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expected, body);
        return JsonDocument.Parse(body).RootElement.Clone();
    }

    public static async Task<HttpStatusCode> DeleteStatusAsync(HttpClient client, string path)
    {
        var response = await client.DeleteAsync(path);
        return response.StatusCode;
    }
}
