namespace telemedicine_webapi.Domain.Entities;
public class PhysicianPatientTransaction : BaseAuditableEntity
{
    public virtual Physician? Physician { get; set; }
    public virtual Patient? Patient { get; set; }
    public virtual TelemedicineService? Service { get; set; }
    public virtual Comment? Comment { get; set; }
}
