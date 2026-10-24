using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using telemedicine_webapi.Infrastructure.Persistence.Context;

namespace telemedicine_webapi.Application.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("JwtSettings:Key", "bkoRhqz4meFCPauYpjyIlZZDCO59rRjrH4IRloPdvg8=");
        builder.UseSetting("JwtSettings:Issuer", "Telemedicine");
        builder.UseSetting("JwtSettings:Audience", "https://localhost");
        builder.UseSetting("DefaultConnection", "Host=localhost;Database=telemedicine_test;Username=postgres;Password=worisoft@123");
        builder.UseSetting("RateLimiting:PartitionHeaderEnabled", "true");
        builder.UseSetting("RateLimiting:Authentication:PermitLimit", "10");
        builder.UseSetting("RateLimiting:Authentication:WindowSeconds", "60");
        builder.UseSetting("Database:ApplyMigrationsOnStartup", "false");
        builder.UseSetting("Consultations:AllowOutsideWindow", "true");
        builder.UseSetting("SeedAdmin", "true");
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var integrationConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            configurationBuilder.AddConfiguration(integrationConfig);
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DefaultConnection"] = integrationConfig["DefaultConnection"],
                ["ConnectionStrings:DefaultConnection"] = integrationConfig.GetConnectionString("DefaultConnection"),
                ["JwtSettings:Key"] = integrationConfig["JwtSettings:Key"],
                ["JwtSettings:Issuer"] = integrationConfig["JwtSettings:Issuer"],
                ["JwtSettings:Audience"] = integrationConfig["JwtSettings:Audience"]
                ,["Database:ApplyMigrationsOnStartup"] = "false"
                ,["SeedAdmin"] = "true"
            });
        });

        builder.ConfigureServices((builder, services) =>
        {
            services.Configure<HubOptions>(options => options.EnableDetailedErrors = true);
            services
                .Remove<DbContextOptions<ApplicationDbContext>>()
                .AddDbContext<ApplicationDbContext>((sp, options) =>
                    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                        builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        });
    }
}
