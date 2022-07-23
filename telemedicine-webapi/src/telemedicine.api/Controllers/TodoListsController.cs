using telemedicine_webapi.Application.TodoLists.Commands.CreateTodoList;
using telemedicine_webapi.Application.TodoLists.Commands.DeleteTodoList;
using telemedicine_webapi.Application.TodoLists.Commands.UpdateTodoList;
using telemedicine_webapi.Application.TodoLists.Queries.ExportTodos;
using telemedicine_webapi.Application.TodoLists.Queries.GetTodos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace telemedicine.api.Controllers;

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
    public async Task<IActionResult> Create(CreateTodoListCommand command)
    {
        var response= await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> Update(UpdateTodoListCommand command)
    {
        var response=await Mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response=await Mediator.Send(new DeleteTodoListCommand(id));
        return Ok(response);
    }
}
