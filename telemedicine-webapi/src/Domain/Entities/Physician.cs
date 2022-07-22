namespace telemedicine_webapi.Domain.Entities;

public class Physician : User
{
    public virtual string? License { get; set; }
    public virtual string? MedicalSpecialization { get; set; }
    public virtual bool IsArchive { get; set; }
    public virtual Hospital? Hospital { get; set; }
    public virtual ICollection<Patient> Patients { get; set; } = new HashSet<Patient>();
}

