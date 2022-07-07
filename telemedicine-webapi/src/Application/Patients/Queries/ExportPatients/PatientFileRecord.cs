using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Patients.Queries.ExportPatients;

public class PatientFileRecord : IMapFrom<Patient>
{
    public string? Title { get; set; }

    public bool Done { get; set; }
}
