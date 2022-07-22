
using Microsoft.EntityFrameworkCore;

namespace telemedicine_webapi.Infrastructure.Persistence.Context;

public interface IApplicationDbContext
{
    DbSet<T> Set<T>() where T:class; 
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
