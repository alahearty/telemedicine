namespace telemedicine_webapi.Domain.Entities;

public class PhysicianAvailability : BaseAuditableEntity
{
    public int PhysicianId { get; set; }
    public virtual Physician Physician { get; set; } = null!;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int SlotDurationMinutes { get; set; } = 30;
    public bool IsActive { get; set; } = true;
}
