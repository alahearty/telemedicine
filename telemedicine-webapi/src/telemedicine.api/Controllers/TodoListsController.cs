using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using telemedicine_webapi.Application.TodoLists.Commands.CreateTodoList;
using telemedicine_webapi.Application.TodoLists.Commands.DeleteTodoList;
using telemedicine_webapi.Application.TodoLists.Commands.UpdateTodoList;
using telemedicine_webapi.Application.TodoLists.Queries.ExportTodos;
using telemedicine_webapi.Application.TodoLists.Queries.GetTodos;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoListsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TodosVm>> Get()
    {
        return await Mediator.Send(new GetTodosQuery());
    }

    [HttpGet("{id}")]
    public async Task<FileResult> Get(int id)
    {
        var vm = await Mediator.Send(new ExportTodosQuery { ListId = id });
        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTodoListCommand command)
    {
        var response = await Mediator.Send(command);
        return HandleResponse(response, StatusCodes.Status201Created);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateTodoListCommand command)
    {
        var response = await Mediator.Send(command);
        return HandleResponse(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await Mediator.Send(new DeleteTodoListCommand(id));
        return HandleResponse(response);
    }
}
