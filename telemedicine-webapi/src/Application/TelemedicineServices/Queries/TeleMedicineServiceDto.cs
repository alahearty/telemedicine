using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.TelemedicineServices.Queries;
public class TeleMedicineServiceDto:IMapFrom<TelemedicineService>
{
    public string? ServiceName { get; set; }
    public decimal Cost { get; set; }
    public string? Description { get; set; }
}
