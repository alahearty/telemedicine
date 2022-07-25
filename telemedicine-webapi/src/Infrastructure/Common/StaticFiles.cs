using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemedicine_webapi.Infrastructure.Common;
public class StaticFiles
{
    public const string SettingName = "SqlServerConnectionSetting";
    public string ConnectionString { get; set; }

}
