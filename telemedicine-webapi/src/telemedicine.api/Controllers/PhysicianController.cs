using Microsoft.AspNetCore.Mvc;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.Physicians.Commands.CreatePhysician;
using telemedicine_webapi.Application.Physicians.Commands.DeletePhysician;
using telemedicine_webapi.Application.Physicians.Commands.PurgePhysician;
using telemedicine_webapi.Application.Physicians.Commands.UpdatePhysician;
using telemedicine_webapi.Application.Physicians.Queries.GetPhysicians;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhysicianController : ApiControllerBase
{
    [HttpGet("{id}")]
    [Permit("Physician, Administrator")]
    public async Task<IActionResult> GetPhysician(int id)
    {
        var query = new GetPhysicianQuery(id);
        var response = await Mediator.Send(query);
        return HandleResponse(response);
    }

    [HttpGet]
    [Permit("Administrator, Physician")]
    public async Task<IActionResult> GetPhysicians()
    {
        var query = new GetPhysiciansQuery();
        var response = await Mediator.Send(query);
        return HandleResponse(response);
    }

    [HttpPost]
    [Permit("Administrator, Physician")]
    public async Task<IActionResult> Create([FromBody] CreatePhysicianCommand command)
    {
        var response = await Mediator.Send(command);
        return HandleResponse(response, StatusCodes.Status201Created);
    }

    [HttpPut("{id}")]
    [Permit("Physician")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePhysicianCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new BaseResponse { NotSuccesful = true }.SetError("ID mismatch"));
        }

        var response = await Mediator.Send(command);
        return HandleResponse(response);
    }

    [HttpDelete("{id}")]
    [Permit("Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await Mediator.Send(new DeletePhysicianCommand(id));
        return HandleResponse(response);
    }

    [HttpDelete("purge")]
    [Permit("Administrator")]
    public async Task<IActionResult> PurgePhysician()
    {
        var response = await Mediator.Send(new PurgePhysicianCommand());
        return HandleResponse(response);
    }
}
