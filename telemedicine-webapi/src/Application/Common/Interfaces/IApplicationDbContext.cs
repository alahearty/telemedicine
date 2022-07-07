
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }
    DbSet<Hospital> Hospitals { get; }

    DbSet<Patient> Patients { get; }

    DbSet<Physician> Physicians { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
