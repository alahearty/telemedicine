using telemedicine_webapi.Application.TodoLists.Queries.ExportTodos;

namespace telemedicine_webapi.Application.Common.Interfaces;

public interface ICsvFileBuilder
{
    byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
}
