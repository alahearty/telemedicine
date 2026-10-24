using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Infrastructure.Persistence.Context;

namespace telemedicine.api.Services;

public static class SeedingDefaultDataExtension
{
    public async static Task SeedData(this IApplicationBuilder app, bool applyMigrations)
    {
        using(var scope=app.ApplicationServices.CreateScope())
        {
            var dbContextInit = scope.ServiceProvider.GetService<IApplicationDbContextInitialiser>();
            if (applyMigrations)
                await dbContextInit!.InitialiseAsync();
            await dbContextInit!.SeedAsync();
        }
    }
}
