using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace telemedicine.api.Services.Authorization;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private const string HAS_PERMISSION_POLICY_NAME = "HasPermission:";
    private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; }

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        BackupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => Task.FromResult(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => Task.FromResult<AuthorizationPolicy>(null);

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(HAS_PERMISSION_POLICY_NAME, StringComparison.OrdinalIgnoreCase))
        {
            var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
            policy.AddRequirements(new PermissionAuthorizationRequirement(policyName.Substring(HAS_PERMISSION_POLICY_NAME.Length)));
            return Task.FromResult(policy.Build());
        }
        return BackupPolicyProvider.GetPolicyAsync(policyName);
    }
}
