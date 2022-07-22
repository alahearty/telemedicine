using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Enums;

namespace telemedicine_webapi.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<BaseResponse> GetUserByEmailAsync(string email);
    Task<string> GetUserNameAsync(int userId);

    Task<string?> GetUserRoleAsync(string email);

    Task<bool> IsInRoleAsync(int userId, string role);

    Task<bool> AuthorizeAsync(int userId, string policyName);

    Task<BaseResponse> CreateUserAsync(string email, string password, string role);

    Task<BaseResponse> DeleteUserAsync(int userId);
}
