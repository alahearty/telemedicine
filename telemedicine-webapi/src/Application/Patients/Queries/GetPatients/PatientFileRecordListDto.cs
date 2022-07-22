using telemedicine_webapi.Application.Common.Mappings;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Patients.Queries.GetPatients;

public class PatientFileRecordListDto : IMapFrom<TodoList>
{
    public PatientFileRecordListDto()
    {
        Items = new List<PatientFileRecordDto>();
    }

    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Colour { get; set; }

    public IList<PatientFileRecordDto> Items { get; set; }
}
