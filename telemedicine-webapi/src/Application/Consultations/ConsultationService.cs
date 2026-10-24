using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Consultations;

public record ConsultationDto(
    Guid SessionId,
    int AppointmentId,
    string Status,
    DateTime ScheduledAtUtc,
    DateTime? PatientJoinedAtUtc,
    DateTime? PhysicianJoinedAtUtc,
    DateTime? StartedAtUtc,
    DateTime? EndedAtUtc,
    string? EndReason,
    string PatientName,
    string PhysicianName,
    bool CallerIsPatient);

public interface IConsultationService
{
    Task<ConsultationDto?> GetForAppointmentAsync(int appointmentId, CancellationToken token = default);
    Task<ConsultationDto> GetOrCreateAsync(int appointmentId, CancellationToken token = default);
    Task<ConsultationDto> GetBySessionAsync(Guid sessionId, CancellationToken token = default);
    Task<ConsultationDto> JoinAsync(Guid sessionId, CancellationToken token = default);
    Task<ConsultationDto> EndAsync(Guid sessionId, string? reason, CancellationToken token = default);
    Task<IReadOnlyList<ConsultationDto>> HistoryAsync(CancellationToken token = default);
    Task EnsureCanSignalAsync(Guid sessionId, CancellationToken token = default);
}

public sealed class ConsultationService : IConsultationService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _user;
    private readonly ConsultationWindowOptions _window;

    public ConsultationService(IApplicationDbContext db, ICurrentUserService user, IOptions<ConsultationWindowOptions> window)
    {
        _db = db;
        _user = user;
        _window = window.Value;
    }

    public async Task<ConsultationDto?> GetForAppointmentAsync(int appointmentId, CancellationToken token = default)
    {
        var appointment = await AuthorizedAppointmentAsync(appointmentId, token);
        var session = await Query().SingleOrDefaultAsync(x => x.AppointmentId == appointment.Id, token);
        return session == null ? null : Map(session);
    }

    public async Task<ConsultationDto> GetOrCreateAsync(int appointmentId, CancellationToken token = default)
    {
        var appointment = await AuthorizedAppointmentAsync(appointmentId, token);
        EnsureEligible(appointment);
        var existing = await Query().SingleOrDefaultAsync(x => x.AppointmentId == appointment.Id, token);
        if (existing != null) return Map(existing);
        EnsureJoinWindow(appointment);

        var session = new ConsultationSession
        {
            AppointmentId = appointment.Id,
            PatientId = appointment.PatientId!.Value,
            PhysicianId = appointment.PhysicianId!.Value,
            ScheduledAtUtc = DateTime.SpecifyKind(appointment.AppointmentDate, DateTimeKind.Utc),
            Created = DateTime.UtcNow,
            CreatedBy = _user.Email,
        };
        _db.ConsultationSessions.Add(session);
        try
        {
            await _db.SaveChangesAsync(token);
        }
        catch (DbUpdateException)
        {
            _db.ConsultationSessions.Remove(session);
            existing = await Query().SingleAsync(x => x.AppointmentId == appointment.Id, token);
            return Map(existing);
        }
        return Map(await Query().SingleAsync(x => x.Id == session.Id, token));
    }

    public async Task<ConsultationDto> GetBySessionAsync(Guid sessionId, CancellationToken token = default) =>
        Map(await AuthorizedSessionAsync(sessionId, token));

    public async Task<ConsultationDto> JoinAsync(Guid sessionId, CancellationToken token = default)
    {
        var session = await AuthorizedSessionAsync(sessionId, token);
        EnsureSignalEligible(session);
        var patient = IsPatient(session);
        var alreadyJoined = patient ? session.PatientJoinedAtUtc.HasValue : session.PhysicianJoinedAtUtc.HasValue;
        if (!alreadyJoined && session.Status != ConsultationStatus.InProgress)
            EnsureJoinWindow(session.Appointment);

        var now = DateTime.UtcNow;
        session.RegisterJoin(patient, now);
        session.LastModified = now;
        session.LastModifiedBy = _user.Email;
        await _db.SaveChangesAsync(token);
        return Map(session);
    }

    public async Task<ConsultationDto> EndAsync(Guid sessionId, string? reason, CancellationToken token = default)
    {
        var session = await AuthorizedSessionAsync(sessionId, token);
        if (session.Status == ConsultationStatus.Completed) return Map(session);
        if (session.Status is ConsultationStatus.Cancelled or ConsultationStatus.Failed)
            throw new InvalidOperationException("The consultation is already closed.");
        session.End(DateTime.UtcNow, reason);
        session.LastModified = session.EndedAtUtc;
        session.LastModifiedBy = _user.Email;
        await _db.SaveChangesAsync(token);
        return Map(session);
    }

    public async Task<IReadOnlyList<ConsultationDto>> HistoryAsync(CancellationToken token = default)
    {
        var query = Query();
        if (Role("Patient"))
        {
            var patient = await _db.Patients.SingleOrDefaultAsync(x => x.IdentityUserId == _user.UserId || x.Email == _user.Email, token)
                ?? throw new ForbiddenAccessException();
            query = query.Where(x => x.PatientId == patient.Id);
        }
        else if (Role("Physician"))
        {
            var physician = await _db.Physicians.SingleOrDefaultAsync(x => x.IdentityUserId == _user.UserId || x.Email == _user.Email, token)
                ?? throw new ForbiddenAccessException();
            query = query.Where(x => x.PhysicianId == physician.Id);
        }
        else throw new ForbiddenAccessException();
        var sessions = await query.OrderByDescending(x => x.ScheduledAtUtc).ToListAsync(token);
        return sessions.Select(Map).ToList();
    }

    public async Task EnsureCanSignalAsync(Guid sessionId, CancellationToken token = default) =>
        EnsureSignalEligible(await AuthorizedSessionAsync(sessionId, token));

    private IQueryable<ConsultationSession> Query() => _db.ConsultationSessions
        .Include(x => x.Appointment).Include(x => x.Patient).Include(x => x.Physician);

    private async Task<Appointment> AuthorizedAppointmentAsync(int id, CancellationToken token)
    {
        var appointment = await _db.Appointments.Include(x => x.Patient).Include(x => x.Physician)
            .SingleOrDefaultAsync(x => x.Id == id, token) ?? throw new NotFoundException("Appointment", id);
        if (!Owns(appointment.Patient, appointment.Physician)) throw new ForbiddenAccessException();
        return appointment;
    }

    private async Task<ConsultationSession> AuthorizedSessionAsync(Guid id, CancellationToken token)
    {
        var session = await Query().SingleOrDefaultAsync(x => x.SessionId == id, token)
            ?? throw new NotFoundException("Consultation", id);
        if (!Owns(session.Patient, session.Physician)) throw new ForbiddenAccessException();
        return session;
    }

    private bool Owns(Patient? patient, Physician? physician) =>
        (Role("Patient") && patient != null && (patient.IdentityUserId == _user.UserId || patient.Email == _user.Email))
        || (Role("Physician") && physician != null && (physician.IdentityUserId == _user.UserId || physician.Email == _user.Email));

    private bool IsPatient(ConsultationSession session) => Role("Patient") &&
        (session.Patient.IdentityUserId == _user.UserId || session.Patient.Email == _user.Email);

    private bool Role(string value) => string.Equals(_user.Role, value, StringComparison.OrdinalIgnoreCase);

    private static void EnsureEligible(Appointment appointment)
    {
        if (appointment.Status != ScheduleStatus.Active || !appointment.PatientId.HasValue || !appointment.PhysicianId.HasValue)
            throw new InvalidOperationException("Only accepted appointments with assigned participants are eligible for consultation.");
    }

    private void EnsureJoinWindow(Appointment appointment)
    {
        if (_window.AllowOutsideWindow) return;
        var now = DateTime.UtcNow;
        if (ConsultationWindow.IsOpen(appointment, _window, now)) return;
        var (opensAtUtc, closesAtUtc) = ConsultationWindow.For(appointment, _window);
        throw new InvalidOperationException(
            $"The video room is available from {opensAtUtc:u} to {closesAtUtc:u} (UTC).");
    }

    private static void EnsureSignalEligible(ConsultationSession session)
    {
        EnsureEligible(session.Appointment);
        if (session.Status is ConsultationStatus.Completed or ConsultationStatus.Cancelled or ConsultationStatus.Failed)
            throw new InvalidOperationException("The consultation is no longer active.");
    }

    private ConsultationDto Map(ConsultationSession x) => new(
        x.SessionId, x.AppointmentId, x.Status.ToString(), x.ScheduledAtUtc,
        x.PatientJoinedAtUtc, x.PhysicianJoinedAtUtc, x.StartedAtUtc, x.EndedAtUtc, x.EndReason,
        $"{x.Patient.FirstName} {x.Patient.LastName}".Trim(),
        $"{x.Physician.FirstName} {x.Physician.LastName}".Trim(), IsPatient(x));
}
