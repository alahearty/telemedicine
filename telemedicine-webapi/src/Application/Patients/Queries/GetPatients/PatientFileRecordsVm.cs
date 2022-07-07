namespace telemedicine_webapi.Application.Patients.Queries.GetTodos;

public class PatientFileRecordsVm
{
    public IList<PriorityLevelDto> PriorityLevels { get; set; } = new List<PriorityLevelDto>();

    public IList<PatientFileRecordListDto> Lists { get; set; } = new List<PatientFileRecordListDto>();
}
