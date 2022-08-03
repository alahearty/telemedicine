using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.Physicians.Commands.CreatePhysician;
using telemedicine_webapi.Application.Physicians.Commands.DeletePhysician;
using telemedicine_webapi.Application.Physicians.Commands.PurgePhysician;
using telemedicine_webapi.Application.Physicians.Commands.UpdatePhysician;
using telemedicine_webapi.Application.Physicians.Queries.GetPhysicians;

namespace telemedicine.api.Controllers;

public class PhysicianController : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPhysician(int id)
    {
        var query = new GetPhysicianQuery(id);
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [Permit("Administrator")]
    [HttpGet("all")]
    public async Task<IActionResult> GetPhysicians()
    {
        var query = new GetPhysiciansQuery();
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [Permit("Administrator, Physician")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePhysicianCommand command)
    {
        var response= await Mediator.Send(command);
        return Ok(response.Data);
    }

    [Permit("Physician")]
    [HttpPut()]
    public async Task<IActionResult> Update([FromBody] UpdatePhysicianCommand command)
    {
        var response=await Mediator.Send(command);
        return Ok(response.Data);
    }

    [Permit("Administrator")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var response=await Mediator.Send(new DeletePhysicianCommand(id));
        return Ok(response.Data);
    }

    [Permit("Administrator")]
    [HttpDelete("purge")]
    public async Task<IActionResult> PurgePhysician()
    {
        var response = await Mediator.Send(new PurgePhysicianCommand());

        return Ok(response.Data);
    }
}
