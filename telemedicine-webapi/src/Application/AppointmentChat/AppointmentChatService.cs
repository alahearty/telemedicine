using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.AppointmentChat;

public sealed class AppointmentChatService : IAppointmentChatService
{
    private static readonly ScheduleStatus[] ChatStatuses =
    [
        ScheduleStatus.Active,
        ScheduleStatus.NotAttended,
        ScheduleStatus.Attended
    ];

    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _user;

    public AppointmentChatService(IApplicationDbContext db, ICurrentUserService user)
    {
        _db = db;
        _user = user;
    }

    public async Task<IReadOnlyList<AppointmentChatThreadDto>> GetThreadsAsync(CancellationToken token = default)
    {
        var actor = await ActorAsync(token);
        var query = _db.Appointments
            .Include(x => x.Patient)
            .Include(x => x.Physician)
            .Include(x => x.TelemedicineService)
            .Where(x => ChatStatuses.Contains(x.Status));

        query = actor.IsPatient
            ? query.Where(x => x.PatientId == actor.ParticipantId)
            : query.Where(x => x.PhysicianId == actor.ParticipantId);

        var appointments = await query.ToListAsync(token);
        if (appointments.Count == 0) return [];

        var appointmentIds = appointments.Select(x => x.Id).ToList();
        var messages = await _db.AppointmentChatMessages
            .Where(x => appointmentIds.Contains(x.AppointmentId))
            .OrderBy(x => x.SentAtUtc)
            .ToListAsync(token);
        var byThread = messages.GroupBy(x => x.AppointmentId).ToDictionary(x => x.Key, x => x.ToList());

        var threads = appointments.Select(appointment =>
        {
            var threadMessages = byThread.TryGetValue(appointment.Id, out var list) ? list : [];
            var last = threadMessages.LastOrDefault();
            var unread = threadMessages.Count(message =>
                !string.Equals(message.SenderRole, actor.Role, StringComparison.OrdinalIgnoreCase)
                && (actor.IsPatient ? message.PatientReadAtUtc is null : message.PhysicianReadAtUtc is null));

            return new AppointmentChatThreadDto(
                appointment.Id,
                actor.IsPatient
                    ? $"{appointment.Physician?.FirstName} {appointment.Physician?.LastName}".Trim()
                    : $"{appointment.Patient?.FirstName} {appointment.Patient?.LastName}".Trim(),
                actor.IsPatient ? "Physician" : "Patient",
                appointment.TelemedicineService?.ServiceName ?? "Consultation",
                appointment.AppointmentDate,
                appointment.ScheduleEndTime,
                appointment.Status.ToString(),
                last?.Body,
                last?.SentAtUtc,
                unread,
                true);
        })
        .OrderByDescending(thread => thread.LastMessageAtUtc.HasValue)
        .ThenByDescending(thread => thread.LastMessageAtUtc)
        .ThenBy(thread => thread.AppointmentDate)
        .ToList();

        return threads;
    }

    public async Task<IReadOnlyList<AppointmentChatMessageDto>> GetMessagesAsync(int appointmentId, CancellationToken token = default)
    {
        var actor = await ActorAsync(token);
        var appointment = await AuthorizedAppointmentAsync(appointmentId, actor, token);
        EnsureChatEligible(appointment);

        var messages = await _db.AppointmentChatMessages
            .Where(x => x.AppointmentId == appointmentId)
            .OrderBy(x => x.SentAtUtc)
            .ThenBy(x => x.Id)
            .ToListAsync(token);

        var now = DateTime.UtcNow;
        var changed = false;
        foreach (var message in messages)
        {
            if (string.Equals(message.SenderRole, actor.Role, StringComparison.OrdinalIgnoreCase)) continue;
            if (actor.IsPatient && message.PatientReadAtUtc is null)
            {
                message.PatientReadAtUtc = now;
                changed = true;
            }
            else if (!actor.IsPatient && message.PhysicianReadAtUtc is null)
            {
                message.PhysicianReadAtUtc = now;
                changed = true;
            }
        }

        if (changed) await _db.SaveChangesAsync(token);
        return messages.Select(message => Map(message, appointment, actor)).ToList();
    }

    public Task<AppointmentChatSendResult> SendAsync(int appointmentId, string body, CancellationToken token = default) =>
        SendInternalAsync(appointmentId, body, null, token);

    public Task<AppointmentChatSendResult> SendAsync(int appointmentId, string body, AppointmentChatIdentity identity, CancellationToken token = default) =>
        SendInternalAsync(appointmentId, body, identity, token);

    private async Task<AppointmentChatSendResult> SendInternalAsync(int appointmentId, string body, AppointmentChatIdentity? identity, CancellationToken token)
    {
        var actor = await ActorAsync(token, identity);
        var appointment = await AuthorizedAppointmentAsync(appointmentId, actor, token);
        EnsureChatEligible(appointment);

        var text = (body ?? string.Empty).Trim();
        if (text.Length == 0) throw new InvalidOperationException("A message is required.");
        if (text.Length > 4000) throw new InvalidOperationException("Messages are limited to 4000 characters.");

        var recipientUserId = actor.IsPatient
            ? appointment.Physician?.IdentityUserId
            : appointment.Patient?.IdentityUserId;
        if (recipientUserId is null || recipientUserId == Guid.Empty)
            throw new ForbiddenAccessException();

        var now = DateTime.UtcNow;
        var message = new AppointmentChatMessage
        {
            AppointmentId = appointment.Id,
            SenderUserId = actor.IdentityUserId,
            SenderRole = actor.Role,
            Body = text,
            SentAtUtc = now,
            PatientReadAtUtc = actor.IsPatient ? now : null,
            PhysicianReadAtUtc = actor.IsPatient ? null : now,
        };
        _db.AppointmentChatMessages.Add(message);
        await _db.SaveChangesAsync(token);

        return new AppointmentChatSendResult(
            Map(message, appointment, actor),
            actor.IdentityUserId,
            recipientUserId.Value,
            appointment.Id);
    }

    public Task MarkReadAsync(int appointmentId, CancellationToken token = default) =>
        MarkReadInternalAsync(appointmentId, null, token);

    public Task MarkReadAsync(int appointmentId, AppointmentChatIdentity identity, CancellationToken token = default) =>
        MarkReadInternalAsync(appointmentId, identity, token);

    private async Task MarkReadInternalAsync(int appointmentId, AppointmentChatIdentity? identity, CancellationToken token)
    {
        var actor = await ActorAsync(token, identity);
        var appointment = await AuthorizedAppointmentAsync(appointmentId, actor, token);
        EnsureChatEligible(appointment);

        var messages = await _db.AppointmentChatMessages
            .Where(x => x.AppointmentId == appointmentId)
            .ToListAsync(token);
        var now = DateTime.UtcNow;
        var changed = false;
        foreach (var message in messages)
        {
            if (string.Equals(message.SenderRole, actor.Role, StringComparison.OrdinalIgnoreCase)) continue;
            if (actor.IsPatient && message.PatientReadAtUtc is null)
            {
                message.PatientReadAtUtc = now;
                changed = true;
            }
            else if (!actor.IsPatient && message.PhysicianReadAtUtc is null)
            {
                message.PhysicianReadAtUtc = now;
                changed = true;
            }
        }

        if (changed) await _db.SaveChangesAsync(token);
    }

    private async Task<ChatActor> ActorAsync(CancellationToken token, AppointmentChatIdentity? identity = null)
    {
        var userId = identity?.UserId ?? _user.UserId;
        var email = identity?.Email ?? _user.Email;
        var role = (identity?.Role ?? _user.Role ?? string.Empty).Trim();
        if (string.Equals(role, "Patient", StringComparison.OrdinalIgnoreCase))
        {
            var patient = await _db.Patients.SingleOrDefaultAsync(x =>
                (userId != Guid.Empty && x.IdentityUserId == userId) || x.Email == email, token)
                ?? throw new ForbiddenAccessException();
            return new ChatActor(true, patient.Id, patient.IdentityUserId ?? userId, "Patient");
        }

        if (string.Equals(role, "Physician", StringComparison.OrdinalIgnoreCase))
        {
            var physician = await _db.Physicians.SingleOrDefaultAsync(x =>
                (userId != Guid.Empty && x.IdentityUserId == userId) || x.Email == email, token)
                ?? throw new ForbiddenAccessException();
            return new ChatActor(false, physician.Id, physician.IdentityUserId ?? userId, "Physician");
        }

        throw new ForbiddenAccessException();
    }

    private async Task<Appointment> AuthorizedAppointmentAsync(int appointmentId, ChatActor actor, CancellationToken token)
    {
        var appointment = await _db.Appointments
            .Include(x => x.Patient)
            .Include(x => x.Physician)
            .Include(x => x.TelemedicineService)
            .SingleOrDefaultAsync(x => x.Id == appointmentId, token)
            ?? throw new NotFoundException("Appointment", appointmentId);

        var owns = actor.IsPatient ? appointment.PatientId == actor.ParticipantId : appointment.PhysicianId == actor.ParticipantId;
        if (!owns) throw new ForbiddenAccessException();
        return appointment;
    }

    private static void EnsureChatEligible(Appointment appointment)
    {
        if (!ChatStatuses.Contains(appointment.Status))
            throw new InvalidOperationException("Chat is only available for accepted appointments.");
    }

    private static AppointmentChatMessageDto Map(AppointmentChatMessage message, Appointment appointment, ChatActor actor) => new(
        message.Id,
        message.AppointmentId,
        message.SenderRole,
        string.Equals(message.SenderRole, "Patient", StringComparison.OrdinalIgnoreCase)
            ? $"{appointment.Patient?.FirstName} {appointment.Patient?.LastName}".Trim()
            : $"{appointment.Physician?.FirstName} {appointment.Physician?.LastName}".Trim(),
        message.Body,
        message.SentAtUtc,
        string.Equals(message.SenderRole, actor.Role, StringComparison.OrdinalIgnoreCase));

    private sealed record ChatActor(bool IsPatient, int ParticipantId, Guid IdentityUserId, string Role);
}

