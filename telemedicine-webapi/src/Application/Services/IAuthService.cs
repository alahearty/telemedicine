using telemedicine_webapi.Application.Common.Models;

namespace telemedicine_webapi.Application.Services;
public interface IAuthService
{
    Task<BaseResponse> Login(string email, string password);
    Task<BaseResponse> Register(string email, string password, string accountType);
}
