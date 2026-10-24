using Microsoft.AspNetCore.Mvc;
using telemedicine.api.Services.Authorization;
using telemedicine_webapi.Application.AppointmentChat;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("api")]
[Permit("Patient, Physician")]
public sealed class AppointmentChatController : ControllerBase
{
    private readonly IAppointmentChatService _chat;

    public AppointmentChatController(IAppointmentChatService chat)
    {
        _chat = chat;
    }

    [HttpGet("chat/threads")]
    public async Task<IActionResult> Threads(CancellationToken token) =>
        Ok(await _chat.GetThreadsAsync(token));

    [HttpGet("appointments/{appointmentId:int}/messages")]
    public async Task<IActionResult> Messages(int appointmentId, CancellationToken token)
    {
        try { return Ok(await _chat.GetMessagesAsync(appointmentId, token)); }
        catch (InvalidOperationException ex) { return ChatUnavailable(ex.Message); }
    }

    [HttpPost("appointments/{appointmentId:int}/messages")]
    public async Task<IActionResult> Send(int appointmentId, [FromBody] SendAppointmentChatRequest? request, CancellationToken token)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new ProblemDetails { Status = 400, Title = "A message is required" });
        if (request.Message.Length > 4000)
            return BadRequest(new ProblemDetails { Status = 400, Title = "Messages are limited to 4000 characters" });

        try
        {
            var result = await _chat.SendAsync(appointmentId, request.Message, token);
            return Ok(result);
        }
        catch (InvalidOperationException ex) { return ChatUnavailable(ex.Message); }
    }

    [HttpPost("appointments/{appointmentId:int}/messages/read")]
    public async Task<IActionResult> MarkRead(int appointmentId, CancellationToken token)
    {
        try
        {
            await _chat.MarkReadAsync(appointmentId, token);
            return Ok();
        }
        catch (InvalidOperationException ex) { return ChatUnavailable(ex.Message); }
    }

    private ConflictObjectResult ChatUnavailable(string detail) => Conflict(new ProblemDetails
    {
        Status = StatusCodes.Status409Conflict,
        Title = "Chat unavailable",
        Detail = detail
    });
}

public sealed record SendAppointmentChatRequest(string Message);
