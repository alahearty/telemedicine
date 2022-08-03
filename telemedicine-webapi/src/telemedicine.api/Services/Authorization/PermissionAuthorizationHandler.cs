using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using telemedicine.api.Controllers;

namespace telemedicine.api.Services.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
    {
        var permissionName = GetPermission(context);
        if (!IsAuthorize(context.User, permissionName)) return Task.CompletedTask;
        context.Succeed(requirement);
        return Task.CompletedTask;
    }

    private bool IsAuthorize(ClaimsPrincipal user, string role)
    {
        string[] allowedRoles = role.Split(new[] {','});
        string userRole = user.Claims.First(claim => claim.Type == "Role").Value;

        bool isAllowed = false;
        foreach (string allowedRole in allowedRoles)
        {
            if (userRole.Trim().Equals(allowedRole.Trim(),StringComparison.InvariantCultureIgnoreCase)) 
                isAllowed = true;
        }
        return isAllowed;
    }

    private string GetPermission(AuthorizationHandlerContext context)
    {
        var httpContext = context.Resource as HttpContext;
        var displayName = httpContext?.GetEndpoint()?.DisplayName;
        var nameComponents = displayName?.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        string controllerName = nameComponents![3];
        var index = nameComponents[4].IndexOf('(');
        string actionName = nameComponents[4].Remove(index - 1);
        var assembly = typeof(Program).Assembly;
        var allControllerTypes = assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(ApiControllerBase)));
        var controllerType = allControllerTypes.FirstOrDefault(x => x.FullName!.Contains(controllerName));
        var actionMethod = controllerType?.GetMethod(actionName);
        var permissionAttribute = actionMethod?.GetCustomAttribute<PermitAttribute>();
        return permissionAttribute!.Name;
    }
}


