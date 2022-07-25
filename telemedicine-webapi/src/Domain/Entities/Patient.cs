using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace telemedicine_webapi.Domain.Entities;

public class Patient : User
{
    public virtual ICollection<Physician> Doctors { get; set; } = new HashSet<Physician>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<HealthAnalysisReport> Analyzes { get; set; } = new List<HealthAnalysisReport>();
}
