using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Entities;
public class TelemedicinePayment : BaseAuditableEntity
{
    public PhysianPatientTransaction? Payment { get; set; }
    public string? Remark { get; set; }
    public bool? IsPaid { get; set; }
}
