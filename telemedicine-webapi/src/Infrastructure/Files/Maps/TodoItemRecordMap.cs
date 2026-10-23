using System.Globalization;
using CsvHelper.Configuration;
using telemedicine_webapi.Application.TodoLists.Queries.ExportTodos;
using telemedicine_webapi.Infrastructure.Files;

namespace telemedicine_webapi.Infrastructure.Files.Maps;

public class TodoItemRecordMap : ClassMap<TodoItemRecord>
{
    public TodoItemRecordMap()
    {
        AutoMap(CultureInfo.InvariantCulture);

        Map(m => m.Done).TypeConverter<YesNoBooleanConverter>();
    }
}
