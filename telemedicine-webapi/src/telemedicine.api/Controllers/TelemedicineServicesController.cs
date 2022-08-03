using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.TelemedicineServices.Commands;
using telemedicine_webapi.Application.TelemedicineServices.Queries;

namespace telemedicine.api.Controllers;

public class TelemedicineServicesController:ApiControllerBase
{
    [Permit("Administrator,Physician")]
    [HttpGet("all")]
    public async Task<IActionResult> GetTelemedicineServices()
    {
        var query = new GetTelemedicineServicesQuery();
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [Permit("Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTelemedicineServiceCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [Permit("Administrator")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await Mediator.Send(new DeleteTelemedicineServiceCommand(id));

        return Ok(response.Data);
    }
}
