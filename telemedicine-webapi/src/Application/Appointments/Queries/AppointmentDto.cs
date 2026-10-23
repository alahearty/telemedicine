using AutoMapper;
using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Appointments.Queries;
public class AppointmentDto : IMapFrom<Appointment>
{
    public int Id { get; set; }
    public int? PatientId { get; set; }
    public string? PatientName { get; set; }
    public int? PhysicianId { get; set; }
    public string? PhysicianName { get; set; }
    public virtual string? ServiceName { get; set; }
    public virtual string? ServiceDescription { get; set; }
    public DateTime? ScheduleStartTime { get; set; }
    public DateTime? AppointmentDate { get; set; }
    public ScheduleStatus Status { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.ServiceName, opt => opt.MapFrom(s =>
                s.TelemedicineService != null ? s.TelemedicineService.ServiceName : null))
            .ForMember(d => d.ServiceDescription, opt => opt.MapFrom(s =>
                s.TelemedicineService != null ? s.TelemedicineService.Description : null))
            .ForMember(d => d.PatientName, opt => opt.MapFrom(s =>
                s.Patient != null ? $"{s.Patient.FirstName} {s.Patient.LastName}".Trim() : null))
            .ForMember(d => d.PhysicianName, opt => opt.MapFrom(s =>
                s.Physician != null ? $"{s.Physician.FirstName} {s.Physician.LastName}".Trim() : null));
    }
}
