using Microsoft.AspNetCore.Mvc;
using telemedicine_webapi.Application.TodoItems.Commands.CreateTodoItem;
using telemedicine_webapi.Application.TodoItems.Commands.DeleteTodoItem;
using telemedicine_webapi.Application.TodoItems.Commands.UpdateTodoItem;
using telemedicine_webapi.Application.TodoItems.Commands.UpdateTodoItemDetail;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoItemsController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTodoItemCommand command)
    {
        var response = await Mediator.Send(command);
        return HandleResponse(response, StatusCodes.Status201Created);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateTodoItemCommand command)
    {
        var response = await Mediator.Send(command);
        return HandleResponse(response);
    }

    [HttpPut("details")]
    public async Task<IActionResult> UpdateItemDetails([FromBody] UpdateTodoItemDetailCommand command)
    {
        var response = await Mediator.Send(command);
        return HandleResponse(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await Mediator.Send(new DeleteTodoItemCommand(id));
        return HandleResponse(response);
    }
}
