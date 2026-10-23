using Application.SignalRHub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using telemedicine_webapi.Application.Common.Interfaces;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("chat-hub")]
[Authorize]
public class ChatHubController : ControllerBase
{
    private readonly IHubContext<ChatHub, IChatHub> _hubContext;
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _user;

    public ChatHubController(
        IHubContext<ChatHub, IChatHub> hubContext,
        IApplicationDbContext db,
        ICurrentUserService user)
    {
        _hubContext = hubContext;
        _db = db;
        _user = user;
    }

    [HttpPost("publish-to-all")]
    public IActionResult SendMessageToAllConnectedClient() => Forbid();

    [HttpPost("send-to-user")]
    public async Task<IActionResult> SendMessageToConnectedClient(
        [FromBody] SendChatMessageRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("A message is required");
        if (request.Message.Length > 4000)
            return BadRequest("Messages are limited to 4000 characters");
        if (string.IsNullOrWhiteSpace(request.RecipientEmail))
            return BadRequest("A recipient email is required");

        var recipientUserId = await ChatAccess.EnsureCanMessageAsync(
            _db, _user.UserId, _user.Email, _user.Role, request.RecipientEmail.Trim(), cancellationToken);

        await _hubContext.Clients.User(recipientUserId.ToString())
            .ReceiveMessageToUser(_user.Email ?? "user", request.Message);
        return Ok();
    }

    [HttpPost("send-file")]
    public async Task<IActionResult> SendFileToConnectedClient(
        [FromForm] string recipientEmail,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest("A file is required");
        if (file.Length > 2 * 1024 * 1024)
            return BadRequest("Files are limited to 2 MB in Release 1");
        var allowed = file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(file.ContentType, "text/plain", StringComparison.OrdinalIgnoreCase);
        if (!allowed)
            return BadRequest("Only images, PDF and plain text files are permitted");

        var recipientUserId = await ChatAccess.EnsureCanMessageAsync(
            _db, _user.UserId, _user.Email, _user.Role, recipientEmail?.Trim() ?? string.Empty, cancellationToken);

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);
        await _hubContext.Clients.User(recipientUserId.ToString()).ReceiveFileMessage(new FileMessage
        {
            FileHeader = $"data: {file.ContentType} ;base64",
            FileBinary = memoryStream.ToArray()
        });
        return Ok();
    }

    [HttpPost("join-group")]
    public IActionResult JoinGroup(string groupName) => Forbid();

    [HttpPut("remove-cleint")]
    public IActionResult RemoveCleintFromGroup(string groupName, string cleintId) => Forbid();

    [HttpPut("exit-group")]
    public IActionResult RemoveFromGroup(string groupName) => Forbid();

    [HttpPost("publish-to-group")]
    public IActionResult SendMessageToGroup(string groupName, string user, string message) => Forbid();

    [HttpGet("connectionId")]
    public IActionResult GetConnectionId() => Ok(new { userId = _user.UserId, email = _user.Email });
}

public record SendChatMessageRequest(string RecipientEmail, string Message);
