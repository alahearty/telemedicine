using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using telemedicine_webapi.Application.Common.Interfaces;

namespace Application.SignalRHub
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>, IChatHubConnection
    {
        private readonly IApplicationDbContext _db;

        public ChatHub(IApplicationDbContext db)
        {
            _db = db;
        }

        public async Task SendMessage(string message)
        {
            // Echo to the authenticated caller. No public broadcast exists in Release 1.
            await Clients.Caller.ReceiveMessage(Context.User?.Identity?.Name ?? "user", message);
        }

        public async Task SendMessageToUser(string recipientEmail, string message)
        {
            var recipientUserId = await ChatAccess.EnsureCanMessageAsync(
                _db,
                UserId,
                Email,
                Role,
                recipientEmail,
                Context.ConnectionAborted);
            await Clients.User(recipientUserId.ToString()).ReceiveMessageToUser(Email ?? "user", message);
        }

        public async Task SendFileMessage(string recipientEmail, FileMessage fileMessage)
        {
            ValidateFile(fileMessage);
            var recipientUserId = await ChatAccess.EnsureCanMessageAsync(
                _db,
                UserId,
                Email,
                Role,
                recipientEmail,
                Context.ConnectionAborted);
            await Clients.User(recipientUserId.ToString()).ReceiveFileMessage(fileMessage);
        }

        public Task JoinGroup(string groupName) =>
            throw new HubException("Conversation groups require server-issued membership");

        public Task RemoveCleintFromGroup(string groupName, string cleintId) =>
            throw new HubException("Conversation groups require server-issued membership");

        public Task RemoveFromGroup(string groupName) =>
            throw new HubException("Conversation groups require server-issued membership");

        public Task SendMessageToGroup(string groupName, string user, string message) =>
            throw new HubException("Conversation groups require server-issued membership");

        public Task SendFileToConnectedClient(string receiverConnectionId, FileDocument choseFile) =>
            throw new HubException("Client-selected connection ids are not permitted");

        public string GetConnectionId() => Context.ConnectionId;

        private Guid UserId =>
            Guid.TryParse(Context.UserIdentifier, out var id) ? id : Guid.Empty;

        private string? Email =>
            Context.User?.FindFirst(ClaimTypes.Email)?.Value
            ?? Context.User?.FindFirst("email")?.Value;

        private string? Role =>
            Context.User?.FindFirst("Role")?.Value
            ?? Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        private static void ValidateFile(FileMessage? fileMessage)
        {
            if (fileMessage?.FileBinary is null || fileMessage.FileBinary.Length == 0)
                throw new HubException("A file payload is required");
            if (fileMessage.FileBinary.Length > 2 * 1024 * 1024)
                throw new HubException("Files are limited to 2 MB in Release 1");
            if (string.IsNullOrWhiteSpace(fileMessage.FileHeader)
                || !(fileMessage.FileHeader.Contains("image/", StringComparison.OrdinalIgnoreCase)
                     || fileMessage.FileHeader.Contains("application/pdf", StringComparison.OrdinalIgnoreCase)
                     || fileMessage.FileHeader.Contains("text/plain", StringComparison.OrdinalIgnoreCase)))
            {
                throw new HubException("Only images, PDF and plain text files are permitted");
            }
        }
    }
}
