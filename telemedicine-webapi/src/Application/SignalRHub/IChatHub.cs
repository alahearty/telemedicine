using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using telemedicine_webapi.Application.AppointmentChat;

namespace Application.SignalRHub
{
    public interface IChatHub
    {
        Task ReceiveMessage(string user, string message);
        Task UserConnected(string connectionId);
        Task ReceiveFileMessage(FileMessage fileMessage);
        Task UserDisconnected(string connectionId);
        Task ReceiveMessageToUser(string user, string message);
        Task ReceiveMessageFromGroup(string user, string message);
        Task ReceiveAppointmentMessage(AppointmentChatMessageDto message);
        Task AppointmentMessagesRead(int appointmentId, string readerRole);
    }

    public interface IChatHubConnection
    {
        string GetConnectionId();
    }

}
