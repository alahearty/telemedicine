using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Entities;
public class Appointment : BaseAuditableEntity
{
    public virtual Physician Physician { get; set; }
    public virtual Patient Patient { get; set; }
    public virtual TelemedicineService TelemedicineService { get; set; }
}
