using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Consultations;

public sealed class ConsultationWindowOptions
{
    public int JoinWindowMinutesBefore { get; set; } = 15;
    public int JoinWindowMinutesAfter { get; set; } = 30;
    public bool AllowOutsideWindow { get; set; }
}

public sealed record ConsultationPolicyDto(
    bool AllowOutsideWindow,
    int JoinWindowMinutesBefore,
    int JoinWindowMinutesAfter,
    DateTime ServerTimeUtc);

public static class ConsultationWindow
{
    public static (DateTime OpensAtUtc, DateTime ClosesAtUtc) For(Appointment appointment, ConsultationWindowOptions options)
    {
        var start = DateTime.SpecifyKind(appointment.AppointmentDate, DateTimeKind.Utc);
        var end = appointment.ScheduleEndTime == default
            ? start.AddMinutes(30)
            : DateTime.SpecifyKind(appointment.ScheduleEndTime, DateTimeKind.Utc);
        return (
            start.AddMinutes(-Math.Max(0, options.JoinWindowMinutesBefore)),
            end.AddMinutes(Math.Max(0, options.JoinWindowMinutesAfter)));
    }

    public static bool IsOpen(Appointment appointment, ConsultationWindowOptions options, DateTime utcNow)
    {
        if (options.AllowOutsideWindow) return true;
        var (opensAtUtc, closesAtUtc) = For(appointment, options);
        return utcNow >= opensAtUtc && utcNow <= closesAtUtc;
    }
}
