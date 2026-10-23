using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.SignalRHub
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ChatHub : Hub<IChatHub>, IChatHubConnection
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.Caller.ReceiveMessage(Context.User?.Identity?.Name ?? "user", message);
        }

        public async Task SendMessageToUser(string receiverConnectionId, string user, string message)
        {
            await Clients.User(receiverConnectionId).ReceiveMessageToUser(Context.User?.Identity?.Name ?? "user", message);
        }

        public async Task SendFileMessage(string receiverConnectionId, FileMessage fileMessage)
        {
            await Clients.User(receiverConnectionId).ReceiveFileMessage(fileMessage);
        }

        public async Task JoinGroup(string groupName)
        {
            throw new HubException("Conversation groups require server-issued membership");
        }
        public async Task RemoveCleintFromGroup(string groupName, string cleintId)
        {
            throw new HubException("Conversation groups require server-issued membership");
        }
        public async Task RemoveFromGroup(string groupName)
        {
            throw new HubException("Conversation groups require server-issued membership");
        }
        public async Task SendMessageToGroup(string groupName, string user, string message)
        {
            throw new HubException("Conversation groups require server-issued membership");
        }
        public async Task SendFileToConnectedClient(string receiverConnectionId, FileDocument choseFile)
        {
            foreach (var file in choseFile.Files)
            {
                if (file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        var fileMessage = new FileMessage
                        {
                            FileHeader = $"data: {file.ContentType} ;base64",
                            FileBinary = memoryStream.ToArray()
                        };

                        await Clients.Client(receiverConnectionId).ReceiveFileMessage(fileMessage);
                    }
                }
            }

        }
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
