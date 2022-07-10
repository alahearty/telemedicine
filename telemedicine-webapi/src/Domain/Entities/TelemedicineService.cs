using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Entities;
public class TelemedicineService : BaseAuditableEntity
{
    public string? ServiceName { get; set; }
    public double Amount { get; set; }
    public string? Description { get; set; }
}
