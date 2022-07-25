using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Infrastructure.Persistence.Context;

namespace telemedicine.api.Services;

public static class SeedingDefaultDataExtension
{
    public async static Task SeedData(this IApplicationBuilder app)
    {
        using(var scope=app.ApplicationServices.CreateScope())
        {
            var dbContextInit = scope.ServiceProvider.GetService<IApplicationDbContextInitialiser>();
            //await dbContextInit!.InitialiseAsync();
            await dbContextInit!.SeedAsync();
        }
    }
}
