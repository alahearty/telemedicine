using Microsoft.AspNetCore.Mvc;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Application.TelemedicineServices.Commands;
using telemedicine_webapi.Application.TelemedicineServices.Queries;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Permit("Administrator")]
public class TelemedicineServicesController : ApiControllerBase
{
    [HttpGet]
    [Permit("Administrator, Physician, Patient")]
    public async Task<IActionResult> GetTelemedicineServices()
    {
        var query = new GetTelemedicineServicesQuery();
        var response = await Mediator.Send(query);
        return HandleResponse(response);
    }

    [HttpGet("{id}")]
    [Permit("Administrator, Physician, Patient")]
    public async Task<IActionResult> GetTelemedicineService(int id)
    {
        var query = new GetTelemedicineServiceByIdQuery(id);
        var response = await Mediator.Send(query);
        return HandleResponse(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTelemedicineServiceCommand command)
    {
        var response = await Mediator.Send(command);
        return HandleResponse(response, StatusCodes.Status201Created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTelemedicineServiceCommand command)
    {
        if (id != command.id)
        {
            return BadRequest(new BaseResponse { NotSuccesful = true }.SetError("ID mismatch"));
        }

        var response = await Mediator.Send(command);
        return HandleResponse(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await Mediator.Send(new DeleteTelemedicineServiceCommand(id));
        return HandleResponse(response);
    }
}
