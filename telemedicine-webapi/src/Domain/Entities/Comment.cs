using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Domain.Entities;
public class Comment : BaseAuditableEntity
{
    public virtual string? Title { get; set; }
    public virtual string? Description { get; set; }
}
