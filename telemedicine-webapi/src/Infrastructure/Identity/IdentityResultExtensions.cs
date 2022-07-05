using telemedicine_webapi.Application.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace telemedicine_webapi.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
}
