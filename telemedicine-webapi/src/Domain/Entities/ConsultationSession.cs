namespace telemedicine_webapi.Domain.Entities;

public class ConsultationSession : BaseAuditableEntity
{
    public Guid SessionId { get; set; } = Guid.NewGuid();
    public int AppointmentId { get; set; }
    public virtual Appointment Appointment { get; set; } = null!;
    public int PatientId { get; set; }
    public virtual Patient Patient { get; set; } = null!;
    public int PhysicianId { get; set; }
    public virtual Physician Physician { get; set; } = null!;
    public ConsultationStatus Status { get; set; } = ConsultationStatus.Scheduled;
    public DateTime ScheduledAtUtc { get; set; }
    public DateTime? PatientJoinedAtUtc { get; set; }
    public DateTime? PhysicianJoinedAtUtc { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? EndedAtUtc { get; set; }
    public string? EndReason { get; set; }

    public void RegisterJoin(bool patient, DateTime utcNow)
    {
        if (Status is ConsultationStatus.Completed or ConsultationStatus.Cancelled or ConsultationStatus.Failed)
            throw new InvalidOperationException("The consultation is already closed.");
        if (utcNow.Kind != DateTimeKind.Utc) throw new ArgumentException("Join timestamps must be UTC.", nameof(utcNow));
        if (patient) PatientJoinedAtUtc ??= utcNow; else PhysicianJoinedAtUtc ??= utcNow;
        Status = PatientJoinedAtUtc.HasValue && PhysicianJoinedAtUtc.HasValue ? ConsultationStatus.InProgress : ConsultationStatus.Ready;
        StartedAtUtc ??= Status == ConsultationStatus.InProgress ? utcNow : null;
    }

    public void End(DateTime utcNow, string? reason)
    {
        if (Status == ConsultationStatus.Completed) return;
        if (Status is ConsultationStatus.Cancelled or ConsultationStatus.Failed)
            throw new InvalidOperationException("The consultation is already closed.");
        if (utcNow.Kind != DateTimeKind.Utc) throw new ArgumentException("End timestamps must be UTC.", nameof(utcNow));
        Status = ConsultationStatus.Completed;
        EndedAtUtc = utcNow;
        var normalized = string.IsNullOrWhiteSpace(reason) ? "Ended by participant" : reason.Trim();
        EndReason = normalized[..Math.Min(normalized.Length, 500)];
    }
}

public enum ConsultationStatus
{
    Scheduled = 0,
    Ready = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    Failed = 5,
}
