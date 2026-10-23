using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Domain.Enums;
using telemedicine_webapi.Infrastructure.Persistence.Context;

namespace telemedicine_webapi.Application.IntegrationTests.Appointments;

using static Testing;

public class PostgresConcurrencyTests : BaseTestFixture
{
    [Test]
    public async Task ExclusionConstraint_AllowsExactlyOneConcurrentOverlappingBooking()
    {
        var patient = new Patient { FirstName = "Test", LastName = "Patient", Email = $"patient-{Guid.NewGuid():N}@test.local", Gender = Gender.Female, Birth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc), Phone = "000", AccountType = AccountType.PATIENT, Created = DateTime.UtcNow };
        var physician = new Physician { FirstName = "Test", LastName = "Doctor", Email = $"doctor-{Guid.NewGuid():N}@test.local", Gender = Gender.Male, Birth = new DateTime(1980, 1, 1, 0, 0, 0, DateTimeKind.Utc), Phone = "000", AccountType = AccountType.PHYSICIAN, License = $"TEST-{Guid.NewGuid():N}", MedicalSpecialization = "Testing", Created = DateTime.UtcNow };
        await AddAsync(patient); await AddAsync(physician);

        var start = DateTime.UtcNow.Date.AddDays(14).AddHours(10);
        using var scope1 = CreateScope(); using var scope2 = CreateScope();
        var db1 = scope1.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var db2 = scope2.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db1.Appointments.Add(NewAppointment(patient.Id, physician.Id, start));
        db2.Appointments.Add(NewAppointment(patient.Id, physician.Id, start.AddMinutes(10)));

        var results = await Task.WhenAll(Save(db1), Save(db2));
        Assert.That(results.Count(x => x), Is.EqualTo(1), "Exactly one overlapping transaction must commit");
        Assert.That(results.Count(x => !x), Is.EqualTo(1), "The database must reject the conflicting transaction");
    }

    private static Appointment NewAppointment(int patientId, int physicianId, DateTime start) => new()
    {
        PatientId = patientId, PhysicianId = physicianId,
        AppointmentDate = DateTime.SpecifyKind(start, DateTimeKind.Utc), ScheduleEndTime = DateTime.SpecifyKind(start.AddMinutes(30), DateTimeKind.Utc),
        Status = ScheduleStatus.Pending, Created = DateTime.UtcNow
    };

    private static async Task<bool> Save(ApplicationDbContext db)
    {
        try { await db.SaveChangesAsync(); return true; }
        catch (DbUpdateException) { return false; }
    }
}
