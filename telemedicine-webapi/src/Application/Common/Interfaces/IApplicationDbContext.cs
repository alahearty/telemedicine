using Microsoft.EntityFrameworkCore;

namespace telemedicine_webapi.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<telemedicine_webapi.Domain.Entities.Patient> Patients { get; }
    DbSet<telemedicine_webapi.Domain.Entities.Physician> Physicians { get; }
    DbSet<telemedicine_webapi.Domain.Entities.PhysicianAvailability> PhysicianAvailabilities { get; }
    DbSet<telemedicine_webapi.Domain.Entities.Appointment> Appointments { get; }
    DbSet<T> Set<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
