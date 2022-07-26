using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Entities;
public class Appointment : BaseAuditableEntity
{
    public virtual Patient? Patient { get; set; }
    public virtual TelemedicineService? TelemedicineService { get; set; }
    public DateTime? ScheduleStartTime => DateTime.UtcNow;
    public virtual ScheduleStatus Status { get; set; }
    public DateTime? ScheduleEndTime { get; set; }
}

public enum ScheduleStatus { Active, NotAttended, Attended}
