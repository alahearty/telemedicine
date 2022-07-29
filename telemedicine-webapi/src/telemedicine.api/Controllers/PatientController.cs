﻿using Microsoft.AspNetCore.Mvc;
using telemedicine_webapi.Application.Patients.Commands.CreatePatients;
using telemedicine_webapi.Application.Patients.Commands.DeletePatients;
using telemedicine_webapi.Application.Patients.Commands.UpdatePatients;
using telemedicine_webapi.Application.Patients.Queries.GetPatients;
namespace telemedicine.api.Controllers;

//[Authorize]
public class PatientController : ApiControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetPatients()
    {
        var query = new GetPatientsQuery();
        var response= await Mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatient(int id)
    {
        var query = new GetPatientQuery(id);
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientCommand command)
    {
        var response= await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] UpdatePatientCommand command)
    {
        var response=await Mediator.Send(command);

        return Ok(response.Data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response=await Mediator.Send(new DeletePatientCommand(id));

        return Ok(response.Data);
    }

    [HttpDelete("purge")]
    public async Task<IActionResult> PurgePatient()
    {
        var response = await Mediator.Send(new PurgePatientCommand());

        return Ok(response.Data);
    }
}
