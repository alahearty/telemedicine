using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using telemedicine_webapi.Application.Consultations;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Domain.Enums;
using telemedicine_webapi.Infrastructure.Persistence.Context;

namespace telemedicine_webapi.Application.IntegrationTests.Consultations;

public class ConsultationIntegrationTests
{
    private Guid _patientUserId;
    private Guid _physicianUserId;
    private Guid _outsiderUserId;
    private Appointment _appointment = null!;
    private ConsultationSession _session = null!;

    [SetUp]
    public async Task SetUp()
    {
        await Testing.ResetState();
        _patientUserId = Guid.NewGuid(); _physicianUserId = Guid.NewGuid(); _outsiderUserId = Guid.NewGuid();
        var patient = new Patient { IdentityUserId = _patientUserId, FirstName = "Video", LastName = "Patient", Email = "video.patient@test.local", Phone = "+10000000101", Birth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc), AccountType = AccountType.PATIENT };
        var physician = new Physician { IdentityUserId = _physicianUserId, FirstName = "Video", LastName = "Physician", Email = "video.physician@test.local", Phone = "+10000000102", Birth = new DateTime(1980, 1, 1, 0, 0, 0, DateTimeKind.Utc), AccountType = AccountType.PHYSICIAN };
        var outsider = new Patient { IdentityUserId = _outsiderUserId, FirstName = "Other", LastName = "Patient", Email = "other.patient@test.local", Phone = "+10000000103", Birth = new DateTime(1991, 1, 1, 0, 0, 0, DateTimeKind.Utc), AccountType = AccountType.PATIENT };
        await Testing.AddAsync(patient); await Testing.AddAsync(physician); await Testing.AddAsync(outsider);
        _appointment = new Appointment { PatientId = patient.Id, PhysicianId = physician.Id, AppointmentDate = DateTime.UtcNow.AddHours(1), ScheduleEndTime = DateTime.UtcNow.AddHours(1.5), Status = ScheduleStatus.Active };
        await Testing.AddAsync(_appointment);
        _session = new ConsultationSession { AppointmentId = _appointment.Id, PatientId = patient.Id, PhysicianId = physician.Id, ScheduledAtUtc = _appointment.AppointmentDate, Created = DateTime.UtcNow };
        await Testing.AddAsync(_session);
    }

    [Test]
    public async Task Concurrent_create_requests_produce_one_session()
    {
        using (var scope = Testing.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.ConsultationSessions.Remove(await db.ConsultationSessions.FindAsync(_session.Id) ?? throw new InvalidOperationException());
            await db.SaveChangesAsync();
        }
        using var firstClient = Client(Testing.CreateAccessToken(_patientUserId, "video.patient@test.local", "Patient"));
        using var secondClient = Client(Testing.CreateAccessToken(_patientUserId, "video.patient@test.local", "Patient"));
        var responses = await Task.WhenAll(
            firstClient.PostAsync($"/api/consultations/appointment/{_appointment.Id}", null),
            secondClient.PostAsync($"/api/consultations/appointment/{_appointment.Id}", null));
        responses.Should().OnlyContain(x => x.StatusCode == HttpStatusCode.OK);
        (await Testing.CountAsync<ConsultationSession>()).Should().Be(1);
    }

    [Test]
    public async Task Rest_lifecycle_is_authorized_idempotent_and_persistent()
    {
        using var patient = Client(Testing.CreateAccessToken(_patientUserId, "video.patient@test.local", "Patient"));
        using var outsider = Client(Testing.CreateAccessToken(_outsiderUserId, "other.patient@test.local", "Patient"));

        var first = await patient.PostAsync($"/api/consultations/appointment/{_appointment.Id}", null);
        var second = await patient.PostAsync($"/api/consultations/appointment/{_appointment.Id}", null);
        first.StatusCode.Should().Be(HttpStatusCode.OK); second.StatusCode.Should().Be(HttpStatusCode.OK);
        (await Testing.CountAsync<ConsultationSession>()).Should().Be(1);
        (await outsider.GetAsync($"/api/consultations/{_session.SessionId}")).StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var end1 = await patient.PostAsJsonAsync($"/api/consultations/{_session.SessionId}/end", new { reason = "Complete" });
        var end2 = await patient.PostAsJsonAsync($"/api/consultations/{_session.SessionId}/end", new { reason = "Duplicate" });
        end1.StatusCode.Should().Be(HttpStatusCode.OK); end2.StatusCode.Should().Be(HttpStatusCode.OK);
        var stored = await Testing.FindAsync<ConsultationSession>(_session.Id);
        stored!.Status.Should().Be(ConsultationStatus.Completed);
        stored.EndReason.Should().Be("Complete");
    }

    [Test]
    public async Task Signaling_requires_auth_membership_and_active_joined_session()
    {
        await using var anonymous = Testing.CreateConsultationHubConnection();
        await FluentActions.Awaiting(() => anonymous.StartAsync()).Should().ThrowAsync<HttpRequestException>();

        await using var outsider = Testing.CreateConsultationHubConnection(Testing.CreateAccessToken(_outsiderUserId, "other.patient@test.local", "Patient"));
        await outsider.StartAsync();
        await FluentActions.Awaiting(() => outsider.InvokeAsync("JoinConsultation", _session.SessionId)).Should().ThrowAsync<HubException>();

        await using var patient = Testing.CreateConsultationHubConnection(Testing.CreateAccessToken(_patientUserId, "video.patient@test.local", "Patient"));
        await FluentActions.Awaiting(() => patient.InvokeAsync("SendOffer", _session.SessionId, new SessionDescriptionMessage("offer", "v=0"))).Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task Authorized_offer_answer_and_ice_are_relayed_with_server_identity_and_end_blocks_signaling()
    {
        (await Testing.FindAsync<ConsultationSession>(_session.Id))!.SessionId.Should().Be(_session.SessionId);
        await using var patient = Testing.CreateConsultationHubConnection(Testing.CreateAccessToken(_patientUserId, "video.patient@test.local", "Patient"));
        await using var physician = Testing.CreateConsultationHubConnection(Testing.CreateAccessToken(_physicianUserId, "video.physician@test.local", "Physician"));
        var offer = new TaskCompletionSource<(string Sender, SessionDescriptionMessage Message)>(TaskCreationOptions.RunContinuationsAsynchronously);
        var answer = new TaskCompletionSource<SessionDescriptionMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
        var ice = new TaskCompletionSource<IceCandidateMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
        physician.On<string, SessionDescriptionMessage>("ReceiveOffer", (sender, message) => offer.TrySetResult((sender, message)));
        patient.On<string, SessionDescriptionMessage>("ReceiveAnswer", (_, message) => answer.TrySetResult(message));
        physician.On<string, IceCandidateMessage>("ReceiveIceCandidate", (_, message) => ice.TrySetResult(message));
        await patient.StartAsync();
        await physician.StartAsync();
        await patient.InvokeAsync("JoinConsultation", _session.SessionId);
        await physician.InvokeAsync("JoinConsultation", _session.SessionId);

        await patient.InvokeAsync("SendOffer", _session.SessionId, new SessionDescriptionMessage("offer", "v=0\r\npatient"));
        var received = await offer.Task.WaitAsync(TimeSpan.FromSeconds(5));
        received.Sender.Should().Be(_patientUserId.ToString());
        await physician.InvokeAsync("SendAnswer", _session.SessionId, new SessionDescriptionMessage("answer", "v=0\r\nphysician"));
        (await answer.Task.WaitAsync(TimeSpan.FromSeconds(5))).Type.Should().Be("answer");
        await patient.InvokeAsync("SendIceCandidate", _session.SessionId, new IceCandidateMessage("candidate:1 1 UDP 1 127.0.0.1 9 typ host", "0", 0, null));
        (await ice.Task.WaitAsync(TimeSpan.FromSeconds(5))).Candidate.Should().StartWith("candidate:");

        await patient.InvokeAsync("EndConsultation", _session.SessionId, "done");
        await FluentActions.Awaiting(() => patient.InvokeAsync("SendOffer", _session.SessionId, new SessionDescriptionMessage("offer", "v=0"))).Should().ThrowAsync<HubException>();
    }

    private static HttpClient Client(string token)
    {
        var client = Testing.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
