namespace telemedicine_webapi.Domain.Entities;

public class AppointmentChatMessage : BaseAuditableEntity
{
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;
    public Guid SenderUserId { get; set; }
    public string SenderRole { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime SentAtUtc { get; set; }
    public DateTime? PatientReadAtUtc { get; set; }
    public DateTime? PhysicianReadAtUtc { get; set; }
}
