﻿using telemedicine_webapi.Application.Common.Interfaces;
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
using telemedicine_webapi.Application.Services;
using telemedicine_webapi.Infrastructure.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        var sqlserverConnection = new StaticFiles();
        var jwtSettings = new JwtSettings();
        configuration.GetSection("JwtSettings").Bind(jwtSettings);
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        configuration.GetSection(StaticFiles.SettingName).Bind(sqlserverConnection);
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("telemedicine_webapiDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetValue<string>("DefaultConnection"), builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IApplicationDbContextInitialiser, ApplicationDbContextInitialiser>();

        services.AddDefaultIdentity<IdentityUser<Guid>>()
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IIdentityService, IdentityService>();

        services.AddScoped<IAuthService, AuthService>();

        //services.AddIdentityServer().AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();
        services.AddMemoryCache();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidIssuer=jwtSettings.Issuer,
                    ValidateAudience = false,
                    ValidAudience=jwtSettings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key!))
                };
            });

        return services;
    }
}
