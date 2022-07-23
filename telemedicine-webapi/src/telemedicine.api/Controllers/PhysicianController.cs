using Microsoft.AspNetCore.Mvc;
using telemedicine_webapi.Application.Hospitals.Commands.CreateHospital;
using telemedicine_webapi.Application.Hospitals.Commands.DeleteHospital;
using telemedicine_webapi.Application.Hospitals.Commands.UpdateHospital;

namespace telemedicine.api.Controllers;

public class PhysicianController : ApiControllerBase
{
    [HttpGet]
    // public async Task<IActionResult> GetPhysiciansWithPagination([FromQuery] GetPhysicianQuery query)
    // {
    //     return await Mediator.Send(query);
    // }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHospitalCommand command)
    {
        var response= await Mediator.Send(command);
        return Ok(response.Data);
    }

    [HttpPut()]
    public async Task<IActionResult> Update([FromBody] UpdateHospitalCommand command)
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
        var response=await Mediator.Send(new DeleteHospitalCommand(id));
        return Ok(response.Data);
    }
}
