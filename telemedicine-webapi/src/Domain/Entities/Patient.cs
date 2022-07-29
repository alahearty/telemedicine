using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace telemedicine_webapi.Domain.Entities;

public class Patient : User
{
    public virtual void AddHealthAnalysisReport(HealthAnalysisReport healthAnalysisReport)
    {
        healthAnalysisReport.Patient=this;
        HealthAnalysisReports.Add(healthAnalysisReport);
    }
    
    public virtual void RemoveHealthAnalysisReport(HealthAnalysisReport healthAnalysisReport)
    {
        HealthAnalysisReports.Remove(healthAnalysisReport);
    }

    public virtual ICollection<HealthAnalysisReport> HealthAnalysisReports { get; set; } = new List<HealthAnalysisReport>();
}
