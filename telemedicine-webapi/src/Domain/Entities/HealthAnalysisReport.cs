using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace telemedicine_webapi.Domain.Entities;

public class HealthAnalysisReport : BaseAuditableEntity
{
    public virtual double Temp { get; set; }
    public virtual int HeartRate { get; set; }
    public virtual string? Pressure { get; set; }
    public virtual DateTime LastMeasurement { get; set; }

    [Required]
    public virtual Patient? Patient { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
