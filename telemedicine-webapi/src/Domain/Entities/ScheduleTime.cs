namespace telemedicine_webapi.Domain.Entities;
public class ScheduleTime : BaseAuditableEntity
{
    public virtual DateTime? StartScheduleTime { get; set; }
    public virtual DateTime? EndScheduleTime { get; set; }
    public virtual Physician? Physician { get; set; }
}
