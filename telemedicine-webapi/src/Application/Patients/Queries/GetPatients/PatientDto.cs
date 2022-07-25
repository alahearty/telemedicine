using AutoMapper;
using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Domain.Enums;

namespace telemedicine_webapi.Application.Patients.Queries.GetPatients;
public class PatientDto:IMapFrom<Patient>
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Patient, PatientDto>()
            .ForMember(d => d.Age, opt => opt.MapFrom(s => s.GetAge()));
    }

}
