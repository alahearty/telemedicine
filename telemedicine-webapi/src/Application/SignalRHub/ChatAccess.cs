using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;

namespace Application.SignalRHub;

/// <summary>
/// Release 1 chat is limited to a patient and that patient's treating
/// physician. There is no general-purpose public broadcast and no client
/// supplied connection id can select a recipient.
/// </summary>
public static class ChatAccess
{
    public static async Task<Guid> EnsureCanMessageAsync(
        IApplicationDbContext db,
        Guid senderUserId,
        string? senderEmail,
        string? senderRole,
        string recipientEmail,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
            throw new ForbiddenAccessException();

        var role = senderRole?.Trim();
        if (string.Equals(role, "Patient", StringComparison.OrdinalIgnoreCase))
        {
            var patient = await db.Patients.FirstOrDefaultAsync(x =>
                (senderUserId != Guid.Empty && x.IdentityUserId == senderUserId) || x.Email == senderEmail, cancellationToken);
            var physician = await db.Physicians.FirstOrDefaultAsync(x => x.Email == recipientEmail, cancellationToken);
            if (patient == null || physician == null)
                throw new ForbiddenAccessException();
            await EnsureTreatingRelationshipAsync(db, patient.Id, physician.Id, cancellationToken);
            return physician.IdentityUserId ?? throw new ForbiddenAccessException();
        }

        if (string.Equals(role, "Physician", StringComparison.OrdinalIgnoreCase))
        {
            var physician = await db.Physicians.FirstOrDefaultAsync(x =>
                (senderUserId != Guid.Empty && x.IdentityUserId == senderUserId) || x.Email == senderEmail, cancellationToken);
            var patient = await db.Patients.FirstOrDefaultAsync(x => x.Email == recipientEmail, cancellationToken);
            if (physician == null || patient == null)
                throw new ForbiddenAccessException();
            await EnsureTreatingRelationshipAsync(db, patient.Id, physician.Id, cancellationToken);
            return patient.IdentityUserId ?? throw new ForbiddenAccessException();
        }

        throw new ForbiddenAccessException();
    }

    private static async Task EnsureTreatingRelationshipAsync(
        IApplicationDbContext db,
        int patientId,
        int physicianId,
        CancellationToken cancellationToken)
    {
        var hasRelationship = await db.Appointments.AnyAsync(x =>
            x.PatientId == patientId
            && x.PhysicianId == physicianId
            && (x.Status == ScheduleStatus.Active
                || x.Status == ScheduleStatus.NotAttended
                || x.Status == ScheduleStatus.Attended), cancellationToken);
        if (!hasRelationship)
            throw new ForbiddenAccessException();
    }
}
