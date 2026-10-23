using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace telemedicine_webapi.Infrastructure.Persistence.Context;

/// <summary>
/// Builds the design-time context through the same Identity service
/// registration used at runtime. Without this, EF Core can generate a model
/// whose Identity column conventions differ from the running application.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    private const string ConnectionString = "Host=localhost;Database=telemedicine;Username=postgres;Password=worisoft@123";

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(ConnectionString, b =>
                b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        services.AddDefaultIdentity<IdentityUser<Guid>>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services.BuildServiceProvider().GetRequiredService<ApplicationDbContext>();
    }
}
