using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Infrastructure.Files;
using telemedicine_webapi.Infrastructure.Identity;
using telemedicine_webapi.Infrastructure.Persistence.Interceptors;
using telemedicine_webapi.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using telemedicine_webapi.Infrastructure.JWTAuthentication;
using telemedicine_webapi.Infrastructure.Persistence.Context;
using telemedicine_webapi.Infrastructure.Persistence.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        var jwtSettings = new JwtSettings();
        configuration.GetSection("JwtSettings").Bind(jwtSettings);
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("telemedicine_webapiDb"));
        }
        else
        {
            var connectionString = configuration.GetValue<string>("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString, builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddDefaultIdentity<ApplicationUser>()
        .AddRoles<UserRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IIdentityService, IdentityService>();

        //services.AddIdentityServer().AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();

        //services.AddAuthentication().AddIdentityServerJwt();

        //services.AddAuthorization(options => options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator")));

        return services;
    }
}
