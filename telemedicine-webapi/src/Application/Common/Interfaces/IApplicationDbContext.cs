using telemedicine_webapi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace telemedicine_webapi.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
