using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using telemedicine_webapi.Application.Common.Interfaces;

namespace telemedicine_webapi.Infrastructure.JWTAuthentication;
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings=jwtSettings.Value;
    }

    public string GenerateJwtToken(int userId, string email, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("Role", role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings?.Key!));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        double tokenDuration=_jwtSettings?.DurationInMinutes??default(double);

        var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings?.Issuer,
                audience: _jwtSettings?.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenDuration),
                signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}
