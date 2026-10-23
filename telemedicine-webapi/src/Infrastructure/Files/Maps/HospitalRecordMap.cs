using System.Globalization;
using CsvHelper.Configuration;
using telemedicine_webapi.Application.Hospitals.Queries.ExportHospitals;

namespace telemedicine_webapi.Infrastructure.Files.Maps;
public class HospitalRecordMap : ClassMap<HospitalRecord>
{
    public HospitalRecordMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
    }
}
