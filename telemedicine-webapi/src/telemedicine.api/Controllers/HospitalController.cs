using Microsoft.AspNetCore.Mvc;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.Hospitals.Commands.CreateHospital;
using telemedicine_webapi.Application.Hospitals.Commands.DeleteHospital;
using telemedicine_webapi.Application.Hospitals.Commands.UpdateHospital;
using telemedicine_webapi.Application.Hospitals.Queries.GetHospitals;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HospitalController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetHospitals()
    {
        var query = new GetHospitalsQuery();
        var response = await Mediator.Send(query);
        return HandleResponse(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetHospital(int id)
    {
        var query = new GetHospitalByIdQuery(id);
        var response = await Mediator.Send(query);
        return HandleResponse(response);
    }

    [HttpPost]
    [Permit("Administrator")]
    public async Task<IActionResult> Create([FromBody] CreateHospitalCommand command)
    {
        var response = await Mediator.Send(command);
        return HandleResponse(response, StatusCodes.Status201Created);
    }

    [HttpPut("{id}")]
    [Permit("Administrator")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHospitalCommand command)
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
        var response = await Mediator.Send(new DeleteHospitalCommand(id));
        return HandleResponse(response);
    }
}
