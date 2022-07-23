using AutoMapper;
using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Hospitals.Queries.GetHospitals;

public class HospitalDto : IMapFrom<Hospital>
{
    public int Id { get; set; }

    public int ListId { get; set; }

    public HospitalDto()
    {
        Doctors = new HashSet<Physician>();
    }
    public string? HospitalName { get; set; }
    public string? Address { get; set; }

    public virtual ICollection<Physician> Doctors { get; set; }

    public bool Done { get; set; }

    public int Priority { get; set; }

    public string? Note { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Hospital, HospitalDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => (int)s.Id));
    }
}
