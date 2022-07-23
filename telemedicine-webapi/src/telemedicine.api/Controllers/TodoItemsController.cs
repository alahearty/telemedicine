using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Application.TodoItems.Commands.CreateTodoItem;
using telemedicine_webapi.Application.TodoItems.Commands.DeleteTodoItem;
using telemedicine_webapi.Application.TodoItems.Commands.UpdateTodoItem;
using telemedicine_webapi.Application.TodoItems.Commands.UpdateTodoItemDetail;
using telemedicine_webapi.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using telemedicine_webapi.Application.Hospitals.Queries.GetTodoItemsWithPagination;

namespace telemedicine.api.Controllers;

//[Authorize]
public class TodoItemsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTodoItemsWithPagination([FromQuery] GetTodoItemsWithPaginationQuery query)
    {
        var response= await Mediator.Send(query);
        return Ok(response);
    }

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
