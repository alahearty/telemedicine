using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Domain.Enums;

namespace telemedicine_webapi.Application.IntegrationTests.Security;

public class SignalRSecurityTests
{
    private readonly Guid _patientUserId = Guid.NewGuid();
    private readonly Guid _physicianUserId = Guid.NewGuid();
    private readonly Guid _otherPhysicianUserId = Guid.NewGuid();

    [SetUp]
    public async Task SetUp()
    {
        await Testing.ResetState();
        var patient = new Patient { IdentityUserId = _patientUserId, FirstName = "Signal", LastName = "Patient", Email = "signal.patient@test.local", Phone = "+10000000001", Birth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc), AccountType = AccountType.PATIENT };
        var physician = new Physician { IdentityUserId = _physicianUserId, FirstName = "Signal", LastName = "Physician", Email = "signal.physician@test.local", Phone = "+10000000002", Birth = new DateTime(1980, 1, 1, 0, 0, 0, DateTimeKind.Utc), AccountType = AccountType.PHYSICIAN };
        var other = new Physician { IdentityUserId = _otherPhysicianUserId, FirstName = "Other", LastName = "Physician", Email = "other.physician@test.local", Phone = "+10000000003", Birth = new DateTime(1985, 1, 1, 0, 0, 0, DateTimeKind.Utc), AccountType = AccountType.PHYSICIAN };
        await Testing.AddAsync(patient);
        await Testing.AddAsync(physician);
        await Testing.AddAsync(other);
        await Testing.AddAsync(new Appointment
        {
            PatientId = patient.Id, PhysicianId = physician.Id,
            AppointmentDate = DateTime.UtcNow.AddDays(1), ScheduleEndTime = DateTime.UtcNow.AddDays(1).AddMinutes(30),
            Status = ScheduleStatus.Active
        });
    }

    [Test]
    public async Task Missing_and_invalid_tokens_are_rejected()
    {
        await using var anonymous = Testing.CreateHubConnection();
        await using var invalid = Testing.CreateHubConnection("not-a-jwt");
        await using var expired = Testing.CreateHubConnection(CreateExpiredToken());
        await FluentActions.Awaiting(() => anonymous.StartAsync()).Should().ThrowAsync<HttpRequestException>();
        await FluentActions.Awaiting(() => invalid.StartAsync()).Should().ThrowAsync<HttpRequestException>();
        await FluentActions.Awaiting(() => expired.StartAsync()).Should().ThrowAsync<HttpRequestException>();
    }

    [Test]
    public async Task Authenticated_participants_connect_exchange_and_reconnect_with_server_identity()
    {
        var patientToken = Testing.CreateAccessToken(_patientUserId, "signal.patient@test.local", "Patient");
        var physicianToken = Testing.CreateAccessToken(_physicianUserId, "signal.physician@test.local", "Physician");
        await using var patient = Testing.CreateHubConnection(patientToken);
        await using var physician = Testing.CreateHubConnection(physicianToken);
        var received = new TaskCompletionSource<(string Sender, string Message)>(TaskCreationOptions.RunContinuationsAsynchronously);
        physician.On<string, string>("ReceiveMessageToUser", (sender, message) => received.TrySetResult((sender, message)));

        await patient.StartAsync();
        await physician.StartAsync();
        await patient.InvokeAsync("SendMessageToUser", "signal.physician@test.local", "protocol-test");
        var message = await received.Task.WaitAsync(TimeSpan.FromSeconds(5));
        message.Sender.Should().Be("signal.patient@test.local");
        message.Message.Should().Be("protocol-test");

        await patient.StopAsync();
        await patient.StartAsync();
        patient.State.Should().Be(HubConnectionState.Connected);
    }

    [Test]
    public async Task Arbitrary_groups_recipients_and_broadcasts_are_rejected()
    {
        var token = Testing.CreateAccessToken(_patientUserId, "signal.patient@test.local", "Patient");
        await using var patient = Testing.CreateHubConnection(token);
        await patient.StartAsync();

        await FluentActions.Awaiting(() => patient.InvokeAsync("JoinGroup", "other-patient-private" )).Should().ThrowAsync<HubException>();
        await FluentActions.Awaiting(() => patient.InvokeAsync("SendMessageToGroup", "other-patient-private", "spoofed", "message")).Should().ThrowAsync<HubException>();
        await FluentActions.Awaiting(() => patient.InvokeAsync("SendMessageToUser", "other.physician@test.local", "message")).Should().ThrowAsync<HubException>();
    }

    private static string CreateExpiredToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bkoRhqz4meFCPauYpjyIlZZDCO59rRjrH4IRloPdvg8="));
        var token = new JwtSecurityToken("Telemedicine", "https://localhost", [new Claim("uid", Guid.NewGuid().ToString())],
            notBefore: DateTime.UtcNow.AddMinutes(-10), expires: DateTime.UtcNow.AddMinutes(-5),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
