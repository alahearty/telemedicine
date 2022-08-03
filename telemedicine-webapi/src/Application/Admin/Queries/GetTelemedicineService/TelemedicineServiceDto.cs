using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Admin.Queries.GetTelemedicineService;
public class TelemedicineServiceDto : IMapFrom<TelemedicineService>
{
    public string? ServiceName { get; set; }
    public decimal Cost { get; set; }
    public string? Description { get; set; }
}
