using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Entities;
public class TelemedicineService : BaseAuditableEntity
{
    public virtual string? ServiceName { get; set; }
    public virtual double Amount { get; set; }
    public virtual string? Description { get; set; }
}
