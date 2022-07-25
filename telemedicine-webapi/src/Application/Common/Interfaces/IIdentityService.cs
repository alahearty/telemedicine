using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<BaseResponse> GetUserByEmailAsync(string email);
    Task<string> GetUserNameAsync(Guid userId);

    Task<string?> GetUserRoleAsync(string email);

    Task<BaseResponse> GetRoleAsync(string roleName);

    Task<bool> IsInRoleAsync(Guid userId, string role);

    Task<bool> AuthorizeAsync(Guid userId, string policyName);

    Task<BaseResponse> CreateUserAsync(string email, string password, string role);

    Task<BaseResponse> DeleteUserAsync(Guid userId);
}
