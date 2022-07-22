using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using telemedicine.api.Extensions;
using telemedicine_webapi.Infrastructure.JWTAuthentication;
using telemedicine_webapi.Infrastructure.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    var environment = builder.Environment.IsDevelopment() ? "Development" : "Production";
    builder.Configuration.AddJsonFile($"appsettings.{environment}.json");
                
    var connectionString=builder.Configuration.GetValue<string>("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseSqlServer(connectionString));

    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

    var jwtSettings = new JwtSettings();
    builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
            
    builder.Services.RegisterServices();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
    builder.Services.AddWebUIServices();
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TeleMedicine API",
                    Version = "V1"
                });
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
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
                            Reference=new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });
            });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    });
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
