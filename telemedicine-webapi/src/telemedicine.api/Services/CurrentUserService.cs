using System.Security.Claims;

using telemedicine_webapi.Application.Common.Interfaces;

namespace telemedicine.api.Services;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        Guid.TryParse(httpContextAccessor.HttpContext?.User?
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value, out var id);
        UserId = id;

        Email = httpContextAccessor.HttpContext?.User?
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value!;
    }

    public Guid UserId{ get; }

    public string Email { get;}
}
