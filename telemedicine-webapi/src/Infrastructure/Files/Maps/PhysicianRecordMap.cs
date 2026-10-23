using System.Globalization;
using CsvHelper.Configuration;
using telemedicine_webapi.Application.Physicians.Queries.ExportPhysicians;
using telemedicine_webapi.Infrastructure.Files;

namespace telemedicine_webapi.Infrastructure.Files.Maps;
public class PhysicianRecordMap : ClassMap<PhysicianRecord>
{
    public PhysicianRecordMap()
    {
        AutoMap(CultureInfo.InvariantCulture);

        Map(m => m.Done).TypeConverter<YesNoBooleanConverter>();
    }
}
