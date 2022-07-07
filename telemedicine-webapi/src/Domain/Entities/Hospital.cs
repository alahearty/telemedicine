﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace telemedicine_webapi.Domain.Entities;

public class Hospital : BaseAuditableEntity
{
    public Hospital()
    {
        Doctors = new HashSet<Physician>();
    }
    public string? HospitalName { get; set; }
    public string? Address { get; set; }

    public virtual ICollection<Physician> Doctors { get; set; }

}
