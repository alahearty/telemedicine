using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Entities;
public class Appointment : BaseAuditableEntity
{
    public virtual Patient? Patient { get; set; }
    public int? PatientId { get; set; }
    public virtual Physician? Physician { get; set; }
    public int? PhysicianId { get; set; }
    public virtual TelemedicineService? TelemedicineService { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string? Notes { get; set; }
    public virtual ScheduleStatus Status { get; set; } = ScheduleStatus.Pending;
    public DateTime ScheduleEndTime { get; set; }
    public string? StatusReason { get; set; }
}

public enum ScheduleStatus { Active = 0, NotAttended = 1, Attended = 2, Cancelled = 3, Pending = 4, Declined = 5 }
