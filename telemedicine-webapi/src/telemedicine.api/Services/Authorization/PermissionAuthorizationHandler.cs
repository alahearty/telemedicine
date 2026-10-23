using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace telemedicine.api.Services.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAuthorizationRequirement requirement)
    {
        if (!context.User.HasClaim(claim => claim.Type == "Role"))
            return Task.CompletedTask;

        // Controllers such as TelemedicineServicesController declare a
        // restrictive class-level policy plus a broader action-level policy.
        // The most specific (last) endpoint Permit is authoritative for the
        // request, so a class-level requirement cannot veto an action-level grant.
        var endpoint = (context.Resource as HttpContext)?.GetEndpoint();
        var effectivePermission = endpoint?
            .Metadata.GetOrderedMetadata<PermitAttribute>()
            .LastOrDefault()?.Name ?? requirement.Permission;

        if (IsAuthorized(context.User, effectivePermission))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }

    private static bool IsAuthorized(ClaimsPrincipal user, string allowedRoles)
    {
        var userRole = user.Claims.First(claim => claim.Type == "Role").Value;
        return allowedRoles
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(allowedRole => userRole.Equals(allowedRole, StringComparison.InvariantCultureIgnoreCase));
    }
}
