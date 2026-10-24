using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.AppointmentChat;

public sealed record AppointmentChatMessageDto(
    int Id,
    int AppointmentId,
    string SenderRole,
    string SenderName,
    string Body,
    DateTime SentAtUtc,
    bool IsMine);

public sealed record AppointmentChatThreadDto(
    int AppointmentId,
    string CounterpartName,
    string CounterpartRole,
    string ServiceName,
    DateTime AppointmentDate,
    DateTime ScheduleEndTime,
    string Status,
    string? LastMessage,
    DateTime? LastMessageAtUtc,
    int UnreadCount,
    bool CanSend);

public sealed record AppointmentChatSendResult(
    AppointmentChatMessageDto Message,
    Guid SenderUserId,
    Guid RecipientUserId,
    int AppointmentId);

public sealed record AppointmentChatIdentity(Guid UserId, string Email, string? Role);

public interface IAppointmentChatService
{
    Task<IReadOnlyList<AppointmentChatThreadDto>> GetThreadsAsync(CancellationToken token = default);
    Task<IReadOnlyList<AppointmentChatMessageDto>> GetMessagesAsync(int appointmentId, CancellationToken token = default);
    Task<AppointmentChatSendResult> SendAsync(int appointmentId, string body, CancellationToken token = default);
    Task<AppointmentChatSendResult> SendAsync(int appointmentId, string body, AppointmentChatIdentity identity, CancellationToken token = default);
    Task MarkReadAsync(int appointmentId, CancellationToken token = default);
    Task MarkReadAsync(int appointmentId, AppointmentChatIdentity identity, CancellationToken token = default);
}
