using System.Security.Claims;

using telemedicine_webapi.Application.Common.Interfaces;

namespace telemedicine.api.Services;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var principal = httpContextAccessor.HttpContext?.User;
        var idValue = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? principal?.FindFirst("sub")?.Value
            ?? principal?.FindFirst("uid")?.Value;
        Guid.TryParse(idValue, out var id);
        UserId = id;

        Email = principal?.FindFirst(ClaimTypes.Email)?.Value
            ?? principal?.FindFirst("email")?.Value!;

        Role = httpContextAccessor.HttpContext?.User?
            .Claims.FirstOrDefault(claim => claim.Type == "Role" || claim.Type == ClaimTypes.Role)?.Value;
    }

    public Guid UserId{ get; }

    public string Email { get;}

    public string? Role { get; }
}
