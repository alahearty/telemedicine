using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace telemedicine_webapi.Domain.Entities;

public class HealthAnalysisReport : BaseAuditableEntity
{
    public double Temp { get; set; }
    public int HartRate { get; set; }
    public Pressure? Pressure { get; set; }
    public DateTime LastMeasurement { get; set; }

    [Required]
    public virtual Patient? Patient { get; set; }

    public virtual ECG? ECG { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
}
