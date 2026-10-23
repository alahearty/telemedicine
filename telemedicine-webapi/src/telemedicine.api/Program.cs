using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using telemedicine.api.Services;
using telemedicine.api.Services.Authorization;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Application.SignalRHub;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.

    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddWebUIServices();
    
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TeleMedicine API",
                    Version = "V1",
                    Description = "A comprehensive telemedicine platform API",
                    Contact = new OpenApiContact
                    {
                        Name = "TeleMedicine Support",
                        Email = "support@telemedicine.com"
                    }
                });
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer Scheme. \r\n\r\n 'Bearer' [space] and then your token in the text input below." +
                    "\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                
                // Include XML comments if available
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    swagger.IncludeXmlComments(xmlPath);
                }
    });

    builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
    builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
    builder.Services.AddAuthorization();

    // Enhanced Controllers with JSON options
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.WriteIndented = true;
        });

    // Health Checks
    builder.Services.AddHealthChecks()
        .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

    // Response Caching
    builder.Services.AddResponseCaching(options =>
    {
        options.MaximumBodySize = 1024;
        options.UseCaseSensitivePaths = false;
    });

    // Memory Cache
    builder.Services.AddMemoryCache();

    // Enhanced CORS Configuration
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
        ?? new[] { "http://localhost:8080", "http://localhost:3000", "http://localhost:5173" };

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DefaultPolicy", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithExposedHeaders("X-Pagination", "X-Total-Count");
        });

        options.AddPolicy("DevelopmentPolicy", policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    // Forwarded Headers for reverse proxy support
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

    // Rate Limiting (if needed, requires additional package)
    // builder.Services.AddRateLimiter(options => { ... });
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    
    // Forwarded Headers
    app.UseForwardedHeaders();

    // Global Exception Handler
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    // Swagger - available in all environments
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeleMedicine API V1");
        c.RoutePrefix = "swagger"; // Swagger UI at /swagger
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
    });

    // In development the Vue CLI dev server proxies same-origin HTTP requests
    // to this API. Redirecting those to HTTPS makes the browser follow a
    // cross-origin redirect and drop the Authorization header.
    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }
    
    // CORS
    app.UseCors(app.Environment.IsDevelopment() ? "DevelopmentPolicy" : "DefaultPolicy");
    
    // Response Caching
    app.UseResponseCaching();
    
    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();
    
    // Health Checks
    app.UseHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    exception = e.Value.Exception?.Message,
                    duration = e.Value.Duration.ToString()
                })
            });
            await context.Response.WriteAsync(result);
        }
    });
    
    app.UseHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });
    
    app.UseHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = _ => false
    });

    // Status Code Pages
    app.UseStatusCodePages(async context =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        var statusCode = context.HttpContext.Response.StatusCode;
        var response = new
        {
            statusCode = statusCode,
            message = statusCode switch
            {
                404 => "Resource not found",
                500 => "Internal server error",
                _ => "An error occurred"
            },
            timestamp = DateTime.UtcNow
        };
        await context.HttpContext.Response.WriteAsJsonAsync(response);
    });

    // Controllers
    app.MapControllers();
    
    // SignalR Hub
    app.MapHub<Application.SignalRHub.ChatHub>("/chat-hub");
    
    await app.SeedData();

    app.Run();
}
