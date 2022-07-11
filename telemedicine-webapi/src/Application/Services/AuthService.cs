using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models.Authentication;

namespace telemedicine_webapi.Application.Services;
public class AuthService : IAuthService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IJwtTokenGenerator jwtTokenGenerator)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public AuthenticationResult Register(string firstName, string lastName, string email, string password)
    {
        //Check if the user already exists
        //Create user (generate unique ID)
        //Create JWT token
        Guid id = Guid.NewGuid();
        var token = _jwtTokenGenerator.GenerateJwtToken(id, firstName, lastName);
        return new AuthenticationResult(id, firstName, lastName, email, token);

    }
    public AuthenticationResult Login(string email, string password)
    {
        return new AuthenticationResult(Guid.NewGuid(), "", "", email, "token");
    }

}
