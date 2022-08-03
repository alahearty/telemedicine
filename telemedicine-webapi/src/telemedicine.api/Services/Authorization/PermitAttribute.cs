using Microsoft.AspNetCore.Authorization;

namespace telemedicine.api.Services.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class PermitAttribute : AuthorizeAttribute
{
    private const string HAS_PERMISSION_POLICY_NAME = "HasPermission:";
    public PermitAttribute(string name) => Name = name;

    public string Name
    {
        get => Policy!.Substring(HAS_PERMISSION_POLICY_NAME.Length);
        set
        {
            Policy = $"{HAS_PERMISSION_POLICY_NAME}{value}";
        }
    }
}
