using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace telemedicine_webapi.Infrastructure.Files;

public class YesNoBooleanConverter : DefaultTypeConverter
{
    public override string ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value == null)
            return string.Empty;
        return (bool)value ? "Yes" : "No";
    }
}
