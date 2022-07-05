using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace telemedicine_webapi.Domain.Entities;

public class ECGData : BaseAuditableEntity
{

    public int RR { get; set; }
    public int Time { get; set; }

    [Required]
    public virtual ECG ECG { get; set; }
}