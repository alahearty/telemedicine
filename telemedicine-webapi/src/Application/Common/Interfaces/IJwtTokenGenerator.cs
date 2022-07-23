namespace telemedicine_webapi.Application.Common.Interfaces;
public interface IJwtTokenGenerator
{
    string GenerateJwtToken(int userId, string email, string role);
}
