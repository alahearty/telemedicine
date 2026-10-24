using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Consultations;

public record SessionDescriptionMessage(string Type, string Sdp);
public record IceCandidateMessage(string Candidate, string? SdpMid, int? SdpMLineIndex, string? UsernameFragment);

public interface IConsultationClient
{
    Task ParticipantJoined(string participantId);
    Task ParticipantLeft(string participantId);
    Task ReceiveOffer(string senderId, SessionDescriptionMessage offer);
    Task ReceiveAnswer(string senderId, SessionDescriptionMessage answer);
    Task ReceiveIceCandidate(string senderId, IceCandidateMessage candidate);
    Task ConsultationEnded(string reason);
}

[Authorize]
public sealed class ConsultationHub : Hub<IConsultationClient>
{
    private readonly IApplicationDbContext _db;

    public ConsultationHub(IApplicationDbContext db) => _db = db;

    public async Task JoinConsultation(Guid sessionId)
    {
        var session = await AuthorizedSession(sessionId);
        EnsureActive(session);
        var now = DateTime.UtcNow;
        session.RegisterJoin(IsPatient(session), now);
        session.LastModified = now;
        session.LastModifiedBy = Email;
        await _db.SaveChangesAsync(Context.ConnectionAborted);
        await Groups.AddToGroupAsync(Context.ConnectionId, Group(sessionId), Context.ConnectionAborted);
        await Clients.Group(Group(sessionId)).ParticipantJoined(UserId);
        Context.Items["consultation"] = sessionId;
    }

    public async Task SendOffer(Guid sessionId, SessionDescriptionMessage offer)
    {
        ValidateDescription(offer, "offer");
        await AuthorizeMessage(sessionId);
        await Clients.OthersInGroup(Group(sessionId)).ReceiveOffer(UserId, offer);
    }

    public async Task SendAnswer(Guid sessionId, SessionDescriptionMessage answer)
    {
        ValidateDescription(answer, "answer");
        await AuthorizeMessage(sessionId);
        await Clients.OthersInGroup(Group(sessionId)).ReceiveAnswer(UserId, answer);
    }

    public async Task SendIceCandidate(Guid sessionId, IceCandidateMessage candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate.Candidate) || candidate.Candidate.Length > 4096)
            throw new HubException("Invalid ICE candidate.");
        await AuthorizeMessage(sessionId);
        await Clients.OthersInGroup(Group(sessionId)).ReceiveIceCandidate(UserId, candidate);
    }

    public async Task LeaveConsultation(Guid sessionId)
    {
        EnsureActive(await AuthorizedSession(sessionId));
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, Group(sessionId), Context.ConnectionAborted);
        await Clients.OthersInGroup(Group(sessionId)).ParticipantLeft(UserId);
        Context.Items.Remove("consultation");
    }

    public async Task EndConsultation(Guid sessionId, string? reason)
    {
        var session = await AuthorizedSession(sessionId);
        if (session.Status != ConsultationStatus.Completed)
        {
            session.End(DateTime.UtcNow, reason);
            session.LastModified = session.EndedAtUtc;
            session.LastModifiedBy = Email;
            await _db.SaveChangesAsync(Context.ConnectionAborted);
        }
        await Clients.Group(Group(sessionId)).ConsultationEnded(session.EndReason ?? "Consultation ended");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.Items.TryGetValue("consultation", out var value) && value is Guid sessionId)
            await Clients.OthersInGroup(Group(sessionId)).ParticipantLeft(UserId);
        await base.OnDisconnectedAsync(exception);
    }

    private async Task AuthorizeMessage(Guid sessionId)
    {
        if (!Context.Items.TryGetValue("consultation", out var joined) || joined is not Guid joinedId || joinedId != sessionId)
            throw new HubException("Join the authorized consultation before signaling.");
        try { EnsureActive(await AuthorizedSession(sessionId)); }
        catch { throw new HubException("Consultation signaling is not authorized."); }
    }

    private static void ValidateDescription(SessionDescriptionMessage? message, string expectedType)
    {
        if (message == null || !string.Equals(message.Type, expectedType, StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrWhiteSpace(message.Sdp) || message.Sdp.Length > 128 * 1024)
            throw new HubException($"Invalid {expectedType} description.");
    }

    private string UserId => Context.UserIdentifier ?? throw new HubException("Authenticated identity is required.");
    private Guid UserGuid => Guid.TryParse(UserId, out var id) ? id : Guid.Empty;
    private string? Email => Context.User?.FindFirst(ClaimTypes.Email)?.Value ?? Context.User?.FindFirst("email")?.Value;
    private string? Role => Context.User?.FindFirst("Role")?.Value ?? Context.User?.FindFirst(ClaimTypes.Role)?.Value;
    private bool IsPatient(ConsultationSession session) => string.Equals(Role, "Patient", StringComparison.OrdinalIgnoreCase)
        && (session.Patient.IdentityUserId == UserGuid || session.Patient.Email == Email);
    private async Task<ConsultationSession> AuthorizedSession(Guid sessionId)
    {
        var session = await _db.ConsultationSessions.Include(x => x.Appointment).Include(x => x.Patient).Include(x => x.Physician)
            .SingleOrDefaultAsync(x => x.SessionId == sessionId, Context.ConnectionAborted)
            ?? throw new HubException("Consultation was not found.");
        var authorized = IsPatient(session) || (string.Equals(Role, "Physician", StringComparison.OrdinalIgnoreCase)
            && (session.Physician.IdentityUserId == UserGuid || session.Physician.Email == Email));
        if (!authorized) throw new HubException("Consultation access is not authorized.");
        return session;
    }
    private static void EnsureActive(ConsultationSession session)
    {
        if (session.Appointment.Status != ScheduleStatus.Active || session.Status is ConsultationStatus.Completed or ConsultationStatus.Cancelled or ConsultationStatus.Failed)
            throw new HubException("Consultation is no longer active.");
    }
    private static string Group(Guid id) => $"consultation:{id:N}";
}
