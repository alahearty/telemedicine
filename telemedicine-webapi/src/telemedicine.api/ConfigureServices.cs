using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using telemedicine.api.Services;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Infrastructure.Persistence;
using telemedicine_webapi.Infrastructure.Persistence.Context;
using telemedicine_webapi.WebUI.Filters;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        services.AddControllersWithViews(options =>
            options.Filters.Add<ApiExceptionFilterAttribute>())
                .AddFluentValidation(x => x.AutomaticValidationEnabled = false);

        services.AddRazorPages();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        //services.AddOpenApiDocument(configure =>
        //{
        //    configure.Title = "sma API";
        //    configure.AddSecurity("JWT", Enumerable.Empty<string>(), new alias.OpenApiSecurityScheme
        //    {
        //        Type = alias.OpenApiSecuritySchemeType.ApiKey,
        //        Name = "Authorization",
        //        In = alias.OpenApiSecurityApiKeyLocation.Header,
        //        Description = "Type into the textbox: Bearer {your JWT token}."
        //    });

        //    configure.OperationProcessors.Add(new ng.AspNetCoreOperationSecurityScopeProcessor("JWT"));
        //});

        return services;
    }
}
