using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Application.Appointments;
using telemedicine_webapi.Application.Appointments.Commands;
using telemedicine_webapi.Application.Appointments.Queries;
using telemedicine_webapi.Application.ScheduleAppointment;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ApiControllerBase
{
    [HttpPost]
    [Permit("Patient")]
    [EnableRateLimiting("scheduling")]
    public async Task<IActionResult> ScheduleAppointment([FromBody] ScheduleAppointmentCommand command)
    {
        var response = await Mediator.Send(command);
        return HandleResponse(response, StatusCodes.Status201Created);
    }
    
    [HttpPost("respond")]
    [Permit("Physician")]
    [EnableRateLimiting("scheduling")]
    public async Task<IActionResult> RespondToScheduledAppointment([FromBody] RespondToScheduledAppointmentCommand command)
    {
        var response = await Mediator.Send(command);
        return HandleResponse(response);
    }

    [HttpGet("unattended")]
    [Permit("Physician, Administrator")]
    public async Task<IActionResult> GetUnattendedAppointments()
    {
        var query = new GetAppointmentsQuery();
        var response = await Mediator.Send(query);
        return HandleResponse(response);
    }

    [HttpGet("{id}")]
    [Permit("Patient, Physician, Administrator")]
    [EnableRateLimiting("scheduling")]
    public async Task<IActionResult> GetAppointmentById(int id)
    {
        var query = new GetAppointmentByIdQuery(id);
        var response = await Mediator.Send(query);
        return HandleResponse(response);
    }

    [HttpGet("patient/{patientId}")]
    [Permit("Patient, Administrator")]
    public async Task<IActionResult> GetPatientAppointments(int patientId)
    {
        var query = new GetPatientAppointmentsQuery(patientId);
        var response = await Mediator.Send(query);
        return HandleResponse(response);
    }

    [HttpGet("physician/{physicianId}")]
    [Permit("Physician, Administrator")]
    public async Task<IActionResult> GetPhysicianAppointments(int physicianId)
    {
        var query = new GetPhysicianAppointmentsQuery(physicianId);
        var response = await Mediator.Send(query);
        return HandleResponse(response);
    }

    [HttpDelete("{id}")]
    [Permit("Patient, Physician, Administrator")]
    [EnableRateLimiting("scheduling")]
    public async Task<IActionResult> CancelAppointment(int id, [FromQuery] string? reason = null)
    {
        var command = new CancelAppointmentCommand(id, reason);
        var response = await Mediator.Send(command);
        return HandleResponse(response);
    }

    [HttpPut("{id}")]
    [Permit("Patient, Physician, Administrator")]
    public async Task<IActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentCommand command)
    {
        command.Id = id;
        var response = await Mediator.Send(command);
        return HandleResponse(response);
    }
}
