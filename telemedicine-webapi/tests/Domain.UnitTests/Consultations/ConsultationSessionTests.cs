using FluentAssertions;
using NUnit.Framework;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Domain.UnitTests.Consultations;

public class ConsultationSessionTests
{
    [Test]
    public void Both_participants_move_session_to_in_progress_once()
    {
        var session = new ConsultationSession();
        var first = DateTime.UtcNow;
        session.RegisterJoin(true, first);
        session.Status.Should().Be(ConsultationStatus.Ready);
        session.RegisterJoin(false, first.AddSeconds(1));
        session.Status.Should().Be(ConsultationStatus.InProgress);
        session.StartedAtUtc.Should().Be(first.AddSeconds(1));
        session.RegisterJoin(true, first.AddSeconds(2));
        session.PatientJoinedAtUtc.Should().Be(first);
    }

    [Test]
    public void End_is_idempotent_and_preserves_first_reason()
    {
        var session = new ConsultationSession(); var ended = DateTime.UtcNow;
        session.End(ended, "completed"); session.End(ended.AddMinutes(1), "duplicate");
        session.Status.Should().Be(ConsultationStatus.Completed);
        session.EndedAtUtc.Should().Be(ended); session.EndReason.Should().Be("completed");
    }

    [Test]
    public void Closed_session_rejects_join() =>
        FluentActions.Invoking(() => new ConsultationSession { Status = ConsultationStatus.Cancelled }.RegisterJoin(true, DateTime.UtcNow))
            .Should().Throw<InvalidOperationException>();

    [Test]
    public void Non_utc_lifecycle_time_is_rejected() =>
        FluentActions.Invoking(() => new ConsultationSession().RegisterJoin(true, DateTime.Now))
            .Should().Throw<ArgumentException>();
}
