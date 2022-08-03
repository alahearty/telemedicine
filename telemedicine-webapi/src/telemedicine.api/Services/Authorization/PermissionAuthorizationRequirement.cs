using Microsoft.AspNetCore.Authorization;

namespace telemedicine.api.Services.Authorization;

public class PermissionAuthorizationRequirement:IAuthorizationRequirement
{
    public PermissionAuthorizationRequirement(string permission)
    {
        Permission = permission;
    }

    public string Permission { get; }
}
