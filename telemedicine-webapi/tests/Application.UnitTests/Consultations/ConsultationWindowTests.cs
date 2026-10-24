using FluentAssertions;
using NUnit.Framework;
using telemedicine_webapi.Application.Consultations;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.UnitTests.Consultations;

public class ConsultationWindowTests
{
    [Test]
    public void Window_opens_before_the_appointment_and_closes_after_the_buffer()
    {
        var start = new DateTime(2026, 10, 29, 9, 0, 0, DateTimeKind.Utc);
        var appointment = new Appointment { AppointmentDate = start, ScheduleEndTime = start.AddMinutes(30) };
        var options = new ConsultationWindowOptions { JoinWindowMinutesBefore = 15, JoinWindowMinutesAfter = 30 };

        var (opensAt, closesAt) = ConsultationWindow.For(appointment, options);

        opensAt.Should().Be(start.AddMinutes(-15));
        closesAt.Should().Be(start.AddMinutes(60));
        ConsultationWindow.IsOpen(appointment, options, start.AddMinutes(-16)).Should().BeFalse();
        ConsultationWindow.IsOpen(appointment, options, start.AddMinutes(-15)).Should().BeTrue();
        ConsultationWindow.IsOpen(appointment, options, start.AddMinutes(45)).Should().BeTrue();
        ConsultationWindow.IsOpen(appointment, options, start.AddMinutes(61)).Should().BeFalse();
    }

    [Test]
    public void Demo_bypass_allows_joining_outside_the_window()
    {
        var start = DateTime.UtcNow.AddDays(2);
        var appointment = new Appointment { AppointmentDate = start, ScheduleEndTime = start.AddMinutes(30) };
        var options = new ConsultationWindowOptions { AllowOutsideWindow = true };

        ConsultationWindow.IsOpen(appointment, options, DateTime.UtcNow).Should().BeTrue();
    }
}
