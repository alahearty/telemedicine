using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using telemedicine_webapi.Application.Physicians.Queries.ExportPhysicians;

namespace telemedicine_webapi.Infrastructure.Files.Maps;
public class PhysicianRecordMap : ClassMap<PhysicianRecord>
{
    public PhysicianRecordMap()
    {
        AutoMap(CultureInfo.InvariantCulture);

        Map(m => m.Done).ConvertUsing(c => c.Done ? "Yes" : "No");
    }
}
