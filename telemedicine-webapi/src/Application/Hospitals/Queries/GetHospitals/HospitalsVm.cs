namespace telemedicine_webapi.Application.Hospitals.Queries.GetHospitals;

public class HospitalsVm
{
    public IList<PriorityLevelDto> PriorityLevels { get; set; } = new List<PriorityLevelDto>();

    public IList<HospitalDto> Lists { get; set; } = new List<HospitalDto>();
}
