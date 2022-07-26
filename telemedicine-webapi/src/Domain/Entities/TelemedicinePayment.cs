using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Entities;
public class TelemedicinePayment : BaseAuditableEntity
{
    public virtual TelemedicineService? Service { get; set; }
    public virtual decimal AmountPaid { get; set; }
    public virtual bool IsPaid { get; set; }
}
