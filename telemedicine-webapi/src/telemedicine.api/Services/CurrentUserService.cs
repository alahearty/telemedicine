using System.Security.Claims;

using telemedicine_webapi.Application.Common.Interfaces;

namespace telemedicine.api.Services;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        Guid defaultId=default(Guid);
        var isValidId=Guid.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Sid),out defaultId);
        if(isValidId) UserId=defaultId;
    }

    public Guid UserId{get;}
}
