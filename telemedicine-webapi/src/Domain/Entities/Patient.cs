using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace telemedicine_webapi.Domain.Entities;

public class Patient : User
{
    public virtual ICollection<HealthAnalysisReport> Comments { get; set; } = new List<HealthAnalysisReport>();
}
