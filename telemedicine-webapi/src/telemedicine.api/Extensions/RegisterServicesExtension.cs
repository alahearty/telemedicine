using Microsoft.AspNetCore.Identity;
using telemedicine.api.Services;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Infrastructure.Identity;
using telemedicine_webapi.Infrastructure.Persistence.Context;
using telemedicine_webapi.Infrastructure.Persistence.Repositories;

namespace telemedicine.api.Extensions;

public static class RegsterServicesExtension
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        AddInfrastructureServices(services);

        AddApplicationServices(services);

        return services;
    }

    private static void AddApplicationServices(IServiceCollection services)
    {
    }

    private static void AddInfrastructureServices(IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddIdentity<ApplicationUser, UserRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IIdentityService, IdentityService>();

        //services.AddScoped<IJwtTokenGenerator,JwtTokenGenerator>();
    }
}
