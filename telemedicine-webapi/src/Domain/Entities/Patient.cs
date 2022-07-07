using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace telemedicine_webapi.Domain.Entities;

public class Patient : BaseAuditableEntity
{
    public long INN { get; set; }
    [MinLength(3)]
    [MaxLength(16)]
    public string FirstName { get; set; }
    [MinLength(3)]
    [MaxLength(16)]
    public string LastName { get; set; }
    [MinLength(3)]
    [MaxLength(16)]
    public string Patronimic { get; set; }

    public Gender Gender { get; set; }

    [Column(TypeName = "Date")]
    public DateTime Birth { get; set; }
    public string Phone { get; set; }
    public Guid DeviceId { get; set; }
    public bool IsArchive { get; set; }

    public Patient()
    {
        Doctors = new HashSet<Physician>();
    }

    public virtual ICollection<Physician> Doctors { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
    public virtual ICollection<HealthAnalysisReport> Analyzes { get; set; }
}
