namespace telemedicine_webapi.Domain.Entities;

public class Physician : User
{
    public virtual string? License { get; set; }
    public virtual string? MedicalSpecialization { get; set; }
    public virtual Hospital? Hospital { get; set; }
    public virtual ICollection<PhysicianAvailability> Availability { get; set; } = new List<PhysicianAvailability>();
}

