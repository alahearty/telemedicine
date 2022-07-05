using System.Globalization;
using telemedicine_webapi.Application.TodoLists.Queries.ExportTodos;
using CsvHelper.Configuration;

namespace telemedicine_webapi.Infrastructure.Files.Maps;

public class TodoItemRecordMap : ClassMap<TodoItemRecord>
{
    public TodoItemRecordMap()
    {
        AutoMap(CultureInfo.InvariantCulture);

        Map(m => m.Done).ConvertUsing(c => c.Done ? "Yes" : "No");
    }
}
