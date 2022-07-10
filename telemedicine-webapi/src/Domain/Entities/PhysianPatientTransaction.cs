using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Entities;
public class PhysianPatientTransaction : BaseAuditableEntity
{
    public Physician? Physician { get; set; }
    public Patient? Patient { get; set; }
    public TelemedicineService? Service { get; set; }
    public Comment? Comment { get; set; }
}
