using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Entities;
public class PhysianPatientTransaction : BaseAuditableEntity
{
    public virtual Physician? Physician { get; set; }
    public virtual Patient? Patient { get; set; }
    public virtual TelemedicineService? Service { get; set; }
    public virtual Comment? Comment { get; set; }
}
