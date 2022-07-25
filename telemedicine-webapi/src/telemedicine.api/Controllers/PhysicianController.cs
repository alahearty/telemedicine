using Microsoft.AspNetCore.Mvc;
using telemedicine_webapi.Application.Physicians.Commands.CreatePhysician;
using telemedicine_webapi.Application.Physicians.Commands.DeletePhysician;
using telemedicine_webapi.Application.Physicians.Commands.UpdatePhysician;

namespace telemedicine.api.Controllers;

public class PhysicianController : ApiControllerBase
{
    //[HttpGet]
    //public async Task<IActionResult> GetPhysiciansWithPagination([FromQuery] GetPhysicianQuery query)
    //{
    //    return await Mediator.Send(query);
    //}

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePhysicianCommand command)
    {
        var response= await Mediator.Send(command);
        return Ok(response.Data);
    }

    [HttpPut()]
    public async Task<IActionResult> Update([FromBody] UpdatePhysicianCommand command)
    {
        var response=await Mediator.Send(command);
        return Ok(response.Data);
    }

    //[HttpPut("[action]")]
    //public async Task<ActionResult> UpdateItemDetails(int id, UpdateTodoItemDetailCommand command)
    //{
    //    if (id != command.Id)
    //    {
    //        return BadRequest();
    //    }

    //    await Mediator.Send(command);

    //    return NoContent();
    //}

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var response=await Mediator.Send(new DeletePhysicianCommand(id));
        return Ok(response.Data);
    }
}
