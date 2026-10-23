using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Domain.Enums;

namespace telemedicine_webapi.Application.Patients.Queries.ExportPatients;

public class PatientFileRecord : IMapFrom<Patient>
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public Gender Gender { get; set; }

    public DateTime Birth { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public AccountType AccountType { get; set; }
}
