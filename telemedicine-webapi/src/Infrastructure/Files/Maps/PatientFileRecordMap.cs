using System.Globalization;
using CsvHelper.Configuration;
using telemedicine_webapi.Application.Patients.Queries.ExportPatients;

namespace telemedicine_webapi.Infrastructure.Files.Maps;
public class PatientFileRecordMap : ClassMap<PatientFileRecord>
{
    public PatientFileRecordMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
    }
}
