using System.Globalization;
using telemedicine_webapi.Application.TodoLists.Queries.ExportTodos;
using CsvHelper.Configuration;
using telemedicine_webapi.Application.Hospitals.Queries.ExportHospitals;

namespace telemedicine_webapi.Infrastructure.Files.Maps;
public class HospitalRecordMap : ClassMap<HospitalRecord>
{
    public HospitalRecordMap()
    {
        AutoMap(CultureInfo.InvariantCulture);

        Map(m => m.Done).ConvertUsing(c => c.Done ? "Yes" : "No");
    }
}