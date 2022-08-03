using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.HealthAnalysisReports.Commands;
using telemedicine_webapi.Application.HealthAnalysisReports.Queries;

namespace telemedicine.api.Controllers;

[Permit("Physician, Administrator")]
public class HealthAnalysisReportController:ApiControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateHealthAnalysisReport([FromBody] CreateHealthAnalysisReportCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("add-comment")]
    public async Task<IActionResult> AddComment([FromBody] AddCommentCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("{patientId}")]
    public async Task<IActionResult> GetAppointments(int patientId)
    {
        var query = new GetHealthAnalysisReportByPatientIdQuery(patientId);
        var response = await Mediator.Send(query);
        return Ok(response);
    }
}
