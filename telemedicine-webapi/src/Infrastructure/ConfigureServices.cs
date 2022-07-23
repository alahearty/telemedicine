using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Infrastructure.Files;
using telemedicine_webapi.Infrastructure.Identity;
using telemedicine_webapi.Infrastructure.Persistence;
using telemedicine_webapi.Infrastructure.Persistence.Interceptors;
using telemedicine_webapi.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using telemedicine_webapi.Infrastructure.JWTAuthentication;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("telemedicine_webapiDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetValue<string>("DefaultConnection"), builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddIdentityServer().AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();

        services.AddAuthentication().AddIdentityServerJwt();

        services.AddAuthorization(options => options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator")));

        return services;
    }
}
