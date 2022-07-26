using AutoMapper;
using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Appointments;
public class AppointmentDto:IMapFrom<Appointment>
{
    public int Id { get; set; }
    public virtual string? ServiceName { get; set; }
    public virtual string? ServiceDescription { get; set; }
    public DateTime? ScheduleStartTime { get; set; }
    public ScheduleStatus Status { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.ServiceName, opt => opt.MapFrom(s => s.TelemedicineService!.ServiceName))
            .ForMember(d => d.ServiceDescription, opt => opt.MapFrom(s => s.TelemedicineService!.Description));
    }
}
