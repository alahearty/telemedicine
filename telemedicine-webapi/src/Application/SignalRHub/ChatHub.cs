using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.SignalRHub
{
    public class ChatHub : Hub<IChatHub>, IChatHubConnection
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }

        public async Task SendMessageToUser(string receiverConnectionId, string user, string message)
        {
            await Clients.Clients(receiverConnectionId).ReceiveMessageToUser(user, message);
        }

        public async Task SendFileMessage(string receiverConnectionId, FileMessage fileMessage)
        {
            await Clients.Client(receiverConnectionId).ReceiveFileMessage(fileMessage);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task RemoveCleintFromGroup(string groupName, string cleintId)
        {
            await Groups.RemoveFromGroupAsync(cleintId, groupName);
        }
        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task SendMessageToGroup(string groupName, string user, string message)
        {
            await Clients.Group(groupName).ReceiveMessageFromGroup(user, message);
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
