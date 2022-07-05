using System.Globalization;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.TodoLists.Queries.ExportTodos;
using telemedicine_webapi.Infrastructure.Files.Maps;
using CsvHelper;

namespace telemedicine_webapi.Infrastructure.Files;

public class CsvFileBuilder : ICsvFileBuilder
{
    public byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records)
    {
        using var memoryStream = new MemoryStream();
        using (var streamWriter = new StreamWriter(memoryStream))
        {
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            csvWriter.Configuration.RegisterClassMap<TodoItemRecordMap>();
            csvWriter.WriteRecords(records);
        }

        return memoryStream.ToArray();
    }
}
