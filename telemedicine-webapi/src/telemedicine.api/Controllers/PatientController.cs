using Microsoft.AspNetCore.Mvc;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Application.Patients.Commands.CreatePatients;
using telemedicine_webapi.Application.Patients.Commands.DeletePatients;
using telemedicine_webapi.Application.Patients.Commands.UpdatePatients;
using telemedicine_webapi.Application.Patients.Queries.GetPatients;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ApiControllerBase
{
    [HttpGet]
    [Permit("Administrator")]
    public async Task<IActionResult> GetPatients()
    {
        var query = new GetPatientsQuery();
        var response = await Mediator.Send(query);
        return HandleResponse(response);
    }

    [HttpGet("me")]
    [Permit("Patient")]
    public async Task<IActionResult> GetCurrentPatient()
    {
        var query = new GetCurrentPatientQuery();
        var response = await Mediator.Send(query);
        return HandleResponse(response);
    }

    [HttpGet("{id}")]
    [Permit("Patient, Administrator, Physician")]
    public async Task<IActionResult> GetPatient(int id)
    {
        var query = new GetPatientQuery(id);
        var response = await Mediator.Send(query);
        return HandleResponse(response);
    }

    [HttpPost]
    [Permit("Patient")]
    public async Task<IActionResult> Create([FromBody] CreatePatientCommand command)
    {
        var response = await Mediator.Send(command);
        return HandleResponse(response, StatusCodes.Status201Created);
    }

    [HttpPut("{id}")]
    [Permit("Patient")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePatientCommand command)
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
        var response = await Mediator.Send(new DeletePatientCommand(id));
        return HandleResponse(response);
    }

    [HttpDelete("purge")]
    [Permit("Administrator")]
    public async Task<IActionResult> PurgePatient()
    {
        var response = await Mediator.Send(new PurgePatientCommand());
        return HandleResponse(response);
    }
}
