using telemedicine_webapi.Application.TodoItems.Commands.CreateTodoItem;
using telemedicine_webapi.Application.TodoItems.Commands.DeleteTodoItem;
using telemedicine_webapi.Application.TodoItems.Commands.UpdateTodoItem;
using telemedicine_webapi.Application.TodoItems.Commands.UpdateTodoItemDetail;
using Microsoft.AspNetCore.Mvc;

namespace telemedicine.api.Controllers;

//[Authorize]
public class TodoItemsController : ApiControllerBase
{
    //[HttpGet]
    //public async Task<IActionResult> GetTodoItemsWithPagination([FromQuery] GetTodoItemsWithPaginationQuery query)
    //{
    //    var response= await Mediator.Send(query);
    //    return Ok(response);
    //}

    [HttpPost]
    public async Task<IActionResult> Create(CreateTodoItemCommand command)
    {
        var response= await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateTodoItemCommand command)
    {
        var response=await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> UpdateItemDetails(UpdateTodoItemDetailCommand command)
    {
        var response=await Mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response=await Mediator.Send(new DeleteTodoItemCommand(id));
        return Ok(response);
    }
}
