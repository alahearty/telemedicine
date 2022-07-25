namespace telemedicine_webapi.Application.Common.Interfaces;
public interface IJwtTokenGenerator
{
    string GenerateJwtToken(Guid userId, string email, string role);
}
