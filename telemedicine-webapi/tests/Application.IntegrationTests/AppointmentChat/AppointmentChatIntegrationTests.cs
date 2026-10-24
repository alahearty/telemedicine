using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using NUnit.Framework;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Domain.Enums;

namespace telemedicine_webapi.Application.IntegrationTests.AppointmentChat;

public class AppointmentChatIntegrationTests
{
    private Guid _patientUserId;
    private Guid _physicianUserId;
    private Guid _outsiderUserId;
    private int _activeAppointmentId;
    private int _pendingAppointmentId;

    [SetUp]
    public async Task SetUp()
    {
        await Testing.ResetState();
        _patientUserId = Guid.NewGuid();
        _physicianUserId = Guid.NewGuid();
        _outsiderUserId = Guid.NewGuid();

        var patient = new Patient
        {
            IdentityUserId = _patientUserId,
            FirstName = "Chat",
            LastName = "Patient",
            Email = "chat.patient@test.local",
            Phone = "+10000000201",
            Birth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            AccountType = AccountType.PATIENT
        };
        var physician = new Physician
        {
            IdentityUserId = _physicianUserId,
            FirstName = "Chat",
            LastName = "Physician",
            Email = "chat.physician@test.local",
            Phone = "+10000000202",
            Birth = new DateTime(1980, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            AccountType = AccountType.PHYSICIAN,
            License = "CHAT-001"
        };
        var outsider = new Patient
        {
            IdentityUserId = _outsiderUserId,
            FirstName = "Other",
            LastName = "Patient",
            Email = "chat.other@test.local",
            Phone = "+10000000203",
            Birth = new DateTime(1991, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            AccountType = AccountType.PATIENT
        };
        await Testing.AddAsync(patient);
        await Testing.AddAsync(physician);
        await Testing.AddAsync(outsider);

        var active = new Appointment
        {
            PatientId = patient.Id,
            PhysicianId = physician.Id,
            AppointmentDate = DateTime.UtcNow.AddHours(1),
            ScheduleEndTime = DateTime.UtcNow.AddHours(1.5),
            Status = ScheduleStatus.Active
        };
        var pending = new Appointment
        {
            PatientId = patient.Id,
            PhysicianId = physician.Id,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            ScheduleEndTime = DateTime.UtcNow.AddDays(1).AddMinutes(30),
            Status = ScheduleStatus.Pending
        };
        await Testing.AddAsync(active);
        await Testing.AddAsync(pending);
        _activeAppointmentId = active.Id;
        _pendingAppointmentId = pending.Id;
    }

    [Test]
    public async Task Threads_messages_and_unread_state_are_scoped_and_persisted()
    {
        using var patient = Client(Testing.CreateAccessToken(_patientUserId, "chat.patient@test.local", "Patient"));
        using var physician = Client(Testing.CreateAccessToken(_physicianUserId, "chat.physician@test.local", "Physician"));
        using var outsider = Client(Testing.CreateAccessToken(_outsiderUserId, "chat.other@test.local", "Patient"));

        var threads = await GetJson(patient, "/api/chat/threads");
        threads.GetArrayLength().Should().Be(1);
        threads[0].GetProperty("appointmentId").GetInt32().Should().Be(_activeAppointmentId);

        var send = await patient.PostAsJsonAsync($"/api/appointments/{_activeAppointmentId}/messages", new { message = "Hello from the chat integration test." });
        send.StatusCode.Should().Be(HttpStatusCode.OK);
        var sentBody = await send.Content.ReadAsStringAsync();
        JsonDocument.Parse(sentBody).RootElement.GetProperty("message").GetProperty("body").GetString()
            .Should().Be("Hello from the chat integration test.");
        (await Testing.CountAsync<AppointmentChatMessage>()).Should().Be(1);

        var physicianThreads = await GetJson(physician, "/api/chat/threads");
        physicianThreads[0].GetProperty("unreadCount").GetInt32().Should().Be(1);

        var physicianMessages = await GetJson(physician, $"/api/appointments/{_activeAppointmentId}/messages");
        physicianMessages.GetArrayLength().Should().Be(1);
        physicianMessages[0].GetProperty("isMine").GetBoolean().Should().BeFalse();
        physicianMessages[0].GetProperty("body").GetString().Should().Be("Hello from the chat integration test.");

        (await outsider.GetAsync($"/api/appointments/{_activeAppointmentId}/messages")).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await patient.GetAsync($"/api/appointments/{_pendingAppointmentId}/messages")).StatusCode.Should().Be(HttpStatusCode.Conflict);

        var tooLong = new string('x', 4001);
        (await patient.PostAsJsonAsync($"/api/appointments/{_activeAppointmentId}/messages", new { message = tooLong })).StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task SignalR_appointment_messages_are_persisted_and_delivered_to_both_participants()
    {
        var patientToken = Testing.CreateAccessToken(_patientUserId, "chat.patient@test.local", "Patient");
        var physicianToken = Testing.CreateAccessToken(_physicianUserId, "chat.physician@test.local", "Physician");
        await using var patientHub = Testing.CreateHubConnection(patientToken);
        await using var physicianHub = Testing.CreateHubConnection(physicianToken);
        var received = new TaskCompletionSource<(string Role, string Body, bool IsMine)>(TaskCreationOptions.RunContinuationsAsynchronously);
        var echoed = new TaskCompletionSource<(string Role, string Body, bool IsMine)>(TaskCreationOptions.RunContinuationsAsynchronously);

        physicianHub.On<JsonElement>("ReceiveAppointmentMessage", (message) =>
            received.TrySetResult((
                message.GetProperty("senderRole").GetString()!,
                message.GetProperty("body").GetString()!,
                message.GetProperty("isMine").GetBoolean())));
        patientHub.On<JsonElement>("ReceiveAppointmentMessage", (message) =>
            echoed.TrySetResult((
                message.GetProperty("senderRole").GetString()!,
                message.GetProperty("body").GetString()!,
                message.GetProperty("isMine").GetBoolean())));

        await patientHub.StartAsync();
        await physicianHub.StartAsync();
        await patientHub.InvokeAsync("SendAppointmentMessage", _activeAppointmentId, "Live chat message.");

        var recipientMessage = await received.Task.WaitAsync(TimeSpan.FromSeconds(5));
        recipientMessage.Role.Should().Be("Patient");
        recipientMessage.Body.Should().Be("Live chat message.");
        recipientMessage.IsMine.Should().BeFalse();
        var senderMessage = await echoed.Task.WaitAsync(TimeSpan.FromSeconds(5));
        senderMessage.IsMine.Should().BeTrue();
        (await Testing.CountAsync<AppointmentChatMessage>()).Should().Be(1);

        await FluentActions.Awaiting(() => patientHub.InvokeAsync("SendAppointmentMessage", _pendingAppointmentId, "blocked"))
            .Should().ThrowAsync<HubException>();
    }

    private static HttpClient Client(string token)
    {
        var client = Testing.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.TryAddWithoutValidation("X-RateLimit-Partition", Guid.NewGuid().ToString("N"));
        return client;
    }

    private static async Task<JsonElement> GetJson(HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, body);
        return JsonDocument.Parse(body).RootElement;
    }
}
