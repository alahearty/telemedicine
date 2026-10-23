using AutoMapper;
using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.HealthAnalysisReports.Queries;
public class CommentDto : IMapFrom<Comment>
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int PhysicianId { get; set; }
    public string? PhysicianName { get; set; }
    public DateTime Created { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Comment, CommentDto>()
            .ForMember(d => d.PhysicianName, opt => opt.MapFrom(s =>
                s.Physician != null
                    ? $"{s.Physician.FirstName} {s.Physician.LastName}".Trim()
                    : null));
    }
}
