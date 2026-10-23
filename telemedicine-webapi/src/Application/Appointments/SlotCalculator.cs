using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Appointments;

public sealed record AppointmentSlot(DateTime StartUtc, DateTime EndUtc, int DurationMinutes);

/// <summary>
/// All release 1 scheduling instants are UTC. Availability rules are recurring
/// UTC weekday intervals; a slot is a half-open [start, end) interval.
/// </summary>
public static class SlotCalculator
{
    public static DateTime AsUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };

    public static IReadOnlyList<AppointmentSlot> Generate(
        IEnumerable<PhysicianAvailability> rules,
        IEnumerable<Appointment> bookings,
        DateTime fromUtc,
        DateTime toUtc,
        DateTime nowUtc)
    {
        var booked = bookings.ToList();
        var result = new List<AppointmentSlot>();
        var from = AsUtc(fromUtc);
        var to = AsUtc(toUtc);
        var now = AsUtc(nowUtc);

        for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
        {
            foreach (var rule in rules.Where(x => x.IsActive && x.DayOfWeek == date.DayOfWeek))
            {
                var ruleStart = date.Add(rule.StartTime);
                var ruleEnd = date.Add(rule.EndTime);
                for (var start = ruleStart; start.AddMinutes(rule.SlotDurationMinutes) <= ruleEnd; start = start.AddMinutes(rule.SlotDurationMinutes))
                {
                    var end = start.AddMinutes(rule.SlotDurationMinutes);
                    if (start < now || start < from || end > to) continue;
                    if (booked.Any(x => Overlaps(x, start, end))) continue;
                    result.Add(new AppointmentSlot(start, end, rule.SlotDurationMinutes));
                }
            }
        }

        return result.OrderBy(x => x.StartUtc).ToList();
    }

    public static bool TryResolve(
        IEnumerable<PhysicianAvailability> rules,
        IEnumerable<Appointment> bookings,
        DateTime requestedStartUtc,
        DateTime nowUtc,
        out AppointmentSlot slot)
    {
        var start = AsUtc(requestedStartUtc);
        var now = AsUtc(nowUtc);
        slot = default!;

        if (start <= now) return false;

        foreach (var rule in rules.Where(x => x.IsActive && x.DayOfWeek == start.DayOfWeek))
        {
            var ruleStart = start.Date.Add(rule.StartTime);
            var duration = TimeSpan.FromMinutes(rule.SlotDurationMinutes);
            if (start < ruleStart) continue;
            var offset = start - ruleStart;
            if (offset.Ticks % duration.Ticks != 0) continue;

            var end = start.Add(duration);
            if (end > start.Date.Add(rule.EndTime)) continue;
            if (bookings.Any(x => Overlaps(x, start, end))) continue;

            slot = new AppointmentSlot(start, end, rule.SlotDurationMinutes);
            return true;
        }

        return false;
    }

    private static bool Overlaps(Appointment appointment, DateTime start, DateTime end) =>
        appointment.AppointmentDate < end && appointment.ScheduleEndTime > start;
}
