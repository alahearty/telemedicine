using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.Appointments;
using telemedicine_webapi.Application.Appointments.Queries;
using telemedicine_webapi.Application.ScheduleAppointment;

namespace telemedicine.api.Controllers;

public class AppointmentController:ApiControllerBase
{
    [HttpPost("create")]
    [Permit("Patient")]
    public async Task<IActionResult> ScheduleAppointment([FromBody] ScheduleAppointmentCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }
    
    [HttpPost("respond")]
    [Permit("Physician")]
    public async Task<IActionResult> RespondToScheduledAppointment([FromBody] RespondToScheduledAppointmentCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("unattended")]
    [Permit("Physician, Administrator")]
    public async Task<IActionResult> GetAppointments()
    {
        var query = new GetAppointmentsQuery();
        var response = await Mediator.Send(query);
        return Ok(response);
    }

}
