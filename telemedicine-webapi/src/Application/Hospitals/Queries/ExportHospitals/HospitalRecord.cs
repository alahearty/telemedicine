using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Hospitals.Queries.ExportHospitals;

public class HospitalRecord : IMapFrom<Hospital>
{
    public string? Description { get; set; }

    public bool Done { get; set; }
}
