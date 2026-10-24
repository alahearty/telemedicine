using telemedicine_webapi.Infrastructure.Identity;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Respawn;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using telemedicine_webapi.Infrastructure.Persistence.Context;

namespace telemedicine_webapi.Application.IntegrationTests;

[SetUpFixture]
public partial class Testing
{
    private static WebApplicationFactory<Program> _factory = null!;
    private static IConfiguration _configuration = null!;
    private static IServiceScopeFactory _scopeFactory = null!;
    private static Checkpoint _checkpoint = null!;
    private static Guid _currentUserId;

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        _factory = new CustomWebApplicationFactory();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();

        _checkpoint = new Checkpoint
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = new[] { "__EFMigrationsHistory" }
        };
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public static Guid GetCurrentUserId()
    {
        return _currentUserId;
    }

    public static IServiceScope CreateScope() => _scopeFactory.CreateScope();

    public static HttpClient CreateClient() => _factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false,
        BaseAddress = new Uri("https://localhost")
    });

    public static HubConnection CreateHubConnection(string? token = null)
    {
        return new HubConnectionBuilder()
            .WithUrl(new Uri("http://localhost/chat-hub"), options =>
            {
                options.Transports = HttpTransportType.LongPolling;
                options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
                if (token != null) options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .Build();
    }

    public static HubConnection CreateConsultationHubConnection(string? token = null)
    {
        return new HubConnectionBuilder()
            .WithUrl(new Uri("http://localhost/consultation-hub"), options =>
            {
                options.Transports = HttpTransportType.LongPolling;
                options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
                if (token != null) options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .Build();
    }

    public static string CreateAccessToken(Guid userId, string email, string role)
    {
        using var scope = _scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>().GenerateJwtToken(userId, email, role);
    }

    //public static async Task<string> RunAsDefaultUserAsync()
    //{
    //    return await RunAsUserAsync("test@local", "Testing1234!", Array.Empty<string>());
    //}

    //public static async Task<string> RunAsAdministratorAsync()
    //{
    //    return await RunAsUserAsync("administrator@local", "Administrator1234!", new[] { "Administrator" });
    //}

    //public static async Task<string> RunAsUserAsync(string userName, string password, string[] roles)
    //{
    //    using var scope = _scopeFactory.CreateScope();

    //    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    //    var user = new ApplicationUser { UserName = userName, Email = userName };

    //    var result = await userManager.CreateAsync(user, password);

    //    if (roles.Any())
    //    {
    //        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    //        foreach (var role in roles)
    //        {
    //            await roleManager.CreateAsync(new IdentityRole(role));
    //        }

    //        await userManager.AddToRolesAsync(user, roles);
    //    }

    //    if (result.Succeeded)
    //    {
    //        _currentUserId = user.Id;

    //        return _currentUserId;
    //    }

    //    var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

    //    throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
    //}

    public static async Task ResetState()
    {
        await using var connection = new Npgsql.NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        await _checkpoint.Reset(connection);

        _currentUserId = Guid.Empty;
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public static async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
    }
}
