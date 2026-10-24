using System.Reflection;
using telemedicine_webapi.Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Application.SignalRHub;
using telemedicine_webapi.Application.AppointmentChat;
using telemedicine_webapi.Application.Consultations;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IChatHubConnection, ChatHub>();
        services.AddScoped<IConsultationService, ConsultationService>();
        services.AddScoped<IAppointmentChatService, AppointmentChatService>();
        services.AddSignalR(options =>
        {
            options.MaximumReceiveMessageSize = 160 * 1024;
            options.MaximumParallelInvocationsPerClient = 1;
            options.StreamBufferCapacity = 10;
        });
        services.AddAutoMapper(_ => { }, Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

        return services;
    }
}
