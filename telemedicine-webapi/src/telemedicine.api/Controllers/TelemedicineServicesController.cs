using Microsoft.AspNetCore.Mvc;
using telemedicine_webapi.Application.TelemedicineServices.Commands;
using telemedicine_webapi.Application.TelemedicineServices.Queries;

namespace telemedicine.api.Controllers;

public class TelemedicineServicesController:ApiControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetTelemedicineServices()
    {
        var query = new GetTelemedicineServicesQuery();
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTelemedicineServiceCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await Mediator.Send(new DeleteTelemedicineServiceCommand(id));

        return Ok(response.Data);
    }
}
