using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.RateLimiting;
using Application.SignalRHub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using telemedicine.api.Services;
using telemedicine.api.Controllers;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Infrastructure.Persistence.Context;
using telemedicine_webapi.Application.Consultations;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebUIServices();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "TeleMedicine API", Version = "V1" });
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header, Description = "Enter the JWT bearer token." });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement { [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = Array.Empty<string>() });
});
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddSingleton<Microsoft.AspNetCore.SignalR.IUserIdProvider, AppUserIdProvider>();
builder.Services.Configure<IceOptions>(builder.Configuration.GetSection("WebRtc:Ice"));
builder.Services.Configure<ConsultationWindowOptions>(builder.Configuration.GetSection("Consultations"));
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: new[] { "live" })
    .AddDbContextCheck<ApplicationDbContext>("postgresql", tags: new[] { "ready" });

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()?.Where(origin => Uri.TryCreate(origin, UriKind.Absolute, out _)).Distinct().ToArray() ?? [];
if (allowedOrigins.Length == 0) throw new InvalidOperationException("Cors:AllowedOrigins must contain at least one explicit origin.");
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy => policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("X-Pagination", "X-Total-Count", "Retry-After")));

var forwardedHeadersEnabled = builder.Configuration.GetValue<bool>("ForwardedHeaders:Enabled");
if (forwardedHeadersEnabled)
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.ForwardLimit = builder.Configuration.GetValue<int?>("ForwardedHeaders:ForwardLimit") ?? 1;
        foreach (var value in builder.Configuration.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>() ?? [])
            if (IPAddress.TryParse(value, out var address)) options.KnownProxies.Add(address);
    });
}

var allowRateLimitPartitionHeader = builder.Configuration.GetValue<bool>("RateLimiting:PartitionHeaderEnabled");
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/problem+json";
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)) context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds).ToString();
        await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails { Status = 429, Title = "Too many requests", Detail = "The request limit was exceeded. Retry later.", Extensions = { ["traceId"] = context.HttpContext.TraceIdentifier } }), token);
    };
    AddPolicy("authentication", "Authentication", 10);
    AddPolicy("scheduling", "Scheduling", 30);
    AddPolicy("payment", "Payment", 10);
    void AddPolicy(string name, string section, int defaultLimit) => options.AddPolicy(name, http => RateLimitPartition.GetFixedWindowLimiter(PartitionKey(http), _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = builder.Configuration.GetValue<int?>($"RateLimiting:{section}:PermitLimit") ?? defaultLimit,
        Window = TimeSpan.FromSeconds(builder.Configuration.GetValue<int?>($"RateLimiting:{section}:WindowSeconds") ?? 60), QueueLimit = 0, AutoReplenishment = true
    }));
    string PartitionKey(HttpContext context)
    {
        // Test-only isolation. Disabled unless explicitly enabled by configuration.
        if (allowRateLimitPartitionHeader
            && context.Request.Headers.TryGetValue("X-RateLimit-Partition", out var explicitPartition)
            && !string.IsNullOrWhiteSpace(explicitPartition))
        {
            return explicitPartition.ToString();
        }

        return context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue("uid")
            ?? context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
    }
});

var app = builder.Build();
if (forwardedHeadersEnabled) app.UseForwardedHeaders();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("GlobalExceptionHandler");
        logger.LogError(context.Features.Get<IExceptionHandlerFeature>()?.Error, "Unhandled request failure. TraceId: {TraceId}", context.TraceIdentifier);
        context.Response.StatusCode = 500;
        await Results.Problem(statusCode: 500, title: "An unexpected error occurred", extensions: new Dictionary<string, object?> { ["traceId"] = context.TraceIdentifier }).ExecuteAsync(context);
    }));
    app.UseHsts();
    app.UseHttpsRedirection();
}
if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeleMedicine API V1"));
}
app.Use(async (context, next) =>
{
    context.Response.Headers.XContentTypeOptions = "nosniff";
    context.Response.Headers.XFrameOptions = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    await next();
});
app.UseCors("Frontend");
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        var sensitive = context.User.Identity?.IsAuthenticated == true || new[] { "/api/auth", "/api/appointment", "/api/patient", "/api/telemedicinepayment", "/api/consultations", "/chat-hub", "/consultation-hub" }.Any(prefix => context.Request.Path.StartsWithSegments(prefix));
        if (sensitive) { context.Response.Headers.CacheControl = "no-store, no-cache"; context.Response.Headers.Pragma = "no-cache"; }
        return Task.CompletedTask;
    });
    await next();
});
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

static Task WriteHealth(HttpContext context, Microsoft.Extensions.Diagnostics.HealthChecks.HealthReport report)
{
    context.Response.ContentType = "application/json";
    return context.Response.WriteAsJsonAsync(new { status = report.Status.ToString(), checks = report.Entries.Select(e => new { name = e.Key, status = e.Value.Status.ToString(), durationMilliseconds = e.Value.Duration.TotalMilliseconds }) });
}
app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = WriteHealth });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready"), ResponseWriter = WriteHealth });
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live"), ResponseWriter = WriteHealth });
app.UseStatusCodePages(async statusContext =>
{
    var response = statusContext.HttpContext.Response;
    if (response.HasStarted || response.ContentLength.HasValue || !string.IsNullOrEmpty(response.ContentType)) return;
    response.ContentType = "application/problem+json";
    await response.WriteAsJsonAsync(new ProblemDetails { Status = response.StatusCode, Title = $"HTTP {response.StatusCode}", Extensions = { ["traceId"] = statusContext.HttpContext.TraceIdentifier } });
});
app.MapControllers();
app.MapHub<ChatHub>("/chat-hub");
app.MapHub<ConsultationHub>("/consultation-hub");
if (builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup") || builder.Configuration.GetValue<bool>("DemoData:Enabled"))
    await app.SeedData(builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"));
app.Run();

public partial class Program { }
