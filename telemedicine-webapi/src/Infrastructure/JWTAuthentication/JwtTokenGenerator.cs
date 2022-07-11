using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using telemedicine_webapi.Application.Common.Interfaces;

namespace telemedicine_webapi.Infrastructure.JWTAuthentication;
public class JwtTokenGenerator : IJwtTokenGenerator
{
    public string GenerateJwtToken(Guid userId, string firstName, string lastName)
    {
        var signingCredentials = new SigningCredentials
            (new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes("super-secret-key")),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, firstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, lastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var securityToken = new JwtSecurityToken(
            issuer: "Telemedicine",
            expires: DateTime.UtcNow.AddDays(1),
            claims: claims,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);

    }
}
