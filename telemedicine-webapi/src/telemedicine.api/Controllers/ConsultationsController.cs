using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.Consultations;

namespace telemedicine.api.Controllers;

public record EndConsultationRequest(string? Reason);
public record IceServerDto(string[] Urls, string? Username, string? Credential);
public sealed class IceOptions
{
    public List<IceServerDto> Servers { get; set; } = [];
}

[ApiController]
[Route("api/consultations")]
[Permit("Patient, Physician")]
public sealed class ConsultationsController : ControllerBase
{
    private readonly IConsultationService _consultations;
    private readonly IceOptions _ice;
    private readonly ConsultationWindowOptions _window;

    public ConsultationsController(
        IConsultationService consultations,
        IOptions<IceOptions> ice,
        IOptions<ConsultationWindowOptions> window)
    {
        _consultations = consultations;
        _ice = ice.Value;
        _window = window.Value;
    }

    [HttpGet("policy")]
    public ActionResult<ConsultationPolicyDto> Policy() => Ok(new ConsultationPolicyDto(
        _window.AllowOutsideWindow,
        _window.JoinWindowMinutesBefore,
        _window.JoinWindowMinutesAfter,
        DateTime.UtcNow));

    [HttpGet("appointment/{appointmentId:int}")]
    public async Task<ActionResult<ConsultationDto?>> GetForAppointment(int appointmentId, CancellationToken token) =>
        Ok(await _consultations.GetForAppointmentAsync(appointmentId, token));

    [HttpPost("appointment/{appointmentId:int}")]
    public async Task<ActionResult<ConsultationDto>> CreateOrGet(int appointmentId, CancellationToken token)
    {
        try { return Ok(await _consultations.GetOrCreateAsync(appointmentId, token)); }
        catch (InvalidOperationException ex) { return Conflict(new ProblemDetails { Status = 409, Title = "Consultation unavailable", Detail = ex.Message }); }
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<ActionResult<ConsultationDto>> Get(Guid sessionId, CancellationToken token) =>
        Ok(await _consultations.GetBySessionAsync(sessionId, token));

    [HttpPost("{sessionId:guid}/join")]
    public async Task<ActionResult<ConsultationDto>> Join(Guid sessionId, CancellationToken token)
    {
        try { return Ok(await _consultations.JoinAsync(sessionId, token)); }
        catch (InvalidOperationException ex) { return Conflict(new ProblemDetails { Status = 409, Title = "Consultation unavailable", Detail = ex.Message }); }
    }

    [HttpPost("{sessionId:guid}/end")]
    public async Task<ActionResult<ConsultationDto>> End(Guid sessionId, EndConsultationRequest request, CancellationToken token)
    {
        try { return Ok(await _consultations.EndAsync(sessionId, request.Reason, token)); }
        catch (InvalidOperationException ex) { return Conflict(new ProblemDetails { Status = 409, Title = "Consultation unavailable", Detail = ex.Message }); }
    }

    [HttpGet("history")]
    public async Task<ActionResult<IReadOnlyList<ConsultationDto>>> History(CancellationToken token) =>
        Ok(await _consultations.HistoryAsync(token));

    [HttpGet("{sessionId:guid}/ice-servers")]
    public async Task<ActionResult<IReadOnlyList<IceServerDto>>> IceServers(Guid sessionId, CancellationToken token)
    {
        await _consultations.GetBySessionAsync(sessionId, token);
        return Ok(_ice.Servers);
    }
}
