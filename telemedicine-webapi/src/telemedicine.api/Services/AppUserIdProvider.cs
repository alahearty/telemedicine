using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace telemedicine.api.Services;

/// <summary>
/// SignalR identity is the immutable ASP.NET Identity id, never a client-supplied
/// connection id or user name.
/// </summary>
public sealed class AppUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("uid")?.Value
            ?? connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? connection.User?.FindFirst("sub")?.Value;
    }
}
