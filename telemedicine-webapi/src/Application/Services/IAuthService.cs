using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Application.Common.Models.Authentication;

namespace telemedicine_webapi.Application.Services;
public interface IAuthService
{
    Task<BaseResponse> Login(string email, string password);
    Task<BaseResponse> Register(RegisterRequest request, CancellationToken cancellationToken = default);
}
