using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.HealthAnalysisReports.Commands;
using telemedicine_webapi.Application.HealthAnalysisReports.Queries;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Permit("Physician, Administrator")]
public class HealthAnalysisReportController : ApiControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateHealthAnalysisReport([FromBody] CreateHealthAnalysisReportCommand command)
    {
        var response = await Mediator.Send(command);
        return HandleResponse(response, StatusCodes.Status201Created);
    }

    [HttpPost("add-comment")]
    public async Task<IActionResult> AddComment([FromBody] AddCommentCommand command)
    {
        var response = await Mediator.Send(command);
        return HandleResponse(response);
    }

    [HttpGet("{patientId}")]
    public async Task<IActionResult> GetHealthAnalysisReportByPatientId(int patientId)
    {
        var query = new GetHealthAnalysisReportByPatientIdQuery(patientId);
        var response = await Mediator.Send(query);
        return HandleResponse(response);
    }
}
