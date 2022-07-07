using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using telemedicine_webapi.Application.Patients.Queries.ExportPatients;

namespace telemedicine_webapi.Infrastructure.Files.Maps;
public class PatientFileRecordMap : ClassMap<PatientFileRecord>
{
    public PatientFileRecordMap()
    {
        AutoMap(CultureInfo.InvariantCulture);

        Map(m => m.Done).ConvertUsing(c => c.Done ? "Yes" : "No");
    }
}