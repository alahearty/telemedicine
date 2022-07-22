using System.Security.Claims;

using telemedicine_webapi.Application.Common.Interfaces;

namespace telemedicine.api.Services;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        int defaultId=default(int);
        var isValidId=int.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Sid),out defaultId);
        if(isValidId) UserId=defaultId;
    }

    public int UserId{get;}
}
