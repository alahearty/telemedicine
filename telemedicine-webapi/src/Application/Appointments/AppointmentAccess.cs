using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Appointments;

public static class AppointmentAccess
{
    public static bool IsAdministrator(ICurrentUserService user) =>
        string.Equals(user.Role, "Administrator", StringComparison.OrdinalIgnoreCase);

    public static bool IsPatient(ICurrentUserService user) =>
        string.Equals(user.Role, "Patient", StringComparison.OrdinalIgnoreCase);

    public static bool IsPhysician(ICurrentUserService user) =>
        string.Equals(user.Role, "Physician", StringComparison.OrdinalIgnoreCase);

    public static async Task EnsureCanAccessAsync(
        IUnitOfWork context,
        ICurrentUserService user,
        Appointment appointment)
    {
        if (IsAdministrator(user))
            return;

        if (IsPatient(user))
        {
            var patient = await context.PatientRepository.FirstOrDefaultAsync(item =>
                (user.UserId != Guid.Empty && item.IdentityUserId == user.UserId)
                || item.Email == user.Email);
            if (patient == null || appointment.PatientId != patient.Id)
                throw new ForbiddenAccessException();
            return;
        }

        if (IsPhysician(user))
        {
            var physician = await context.PhysicianRepository.FirstOrDefaultAsync(item =>
                (user.UserId != Guid.Empty && item.IdentityUserId == user.UserId)
                || item.Email == user.Email);
            if (physician == null || appointment.PhysicianId != physician.Id)
                throw new ForbiddenAccessException();
            return;
        }

        throw new ForbiddenAccessException();
    }

    public static async Task<Patient> EnsureCurrentPatientAsync(
        IUnitOfWork context,
        ICurrentUserService user)
    {
        var patient = await context.PatientRepository.FirstOrDefaultAsync(item =>
            (user.UserId != Guid.Empty && item.IdentityUserId == user.UserId)
            || item.Email == user.Email);
        if (patient == null)
            throw new ForbiddenAccessException();
        return patient;
    }

    public static async Task<Physician> EnsureCurrentPhysicianAsync(
        IUnitOfWork context,
        ICurrentUserService user)
    {
        var physician = await context.PhysicianRepository.FirstOrDefaultAsync(item =>
            (user.UserId != Guid.Empty && item.IdentityUserId == user.UserId)
            || item.Email == user.Email);
        if (physician == null)
            throw new ForbiddenAccessException();
        return physician;
    }
}
