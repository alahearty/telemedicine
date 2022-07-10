
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using telemedicine_webapi.Application.Common.Models.Authentication;
using telemedicine_webapi.Application.Services;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authenticationService;

    public AuthController(IAuthService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        var requestResult = _authenticationService.Register
        (
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password
        );
        var authResponse = new AuthenticationResponse
        (
            requestResult.Id,
            requestResult.FirstName,
            requestResult.LastName,
            requestResult.Email,
            requestResult.Token
        );

        return Ok(authResponse);
    }
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var loginResult = _authenticationService.Login
       (
           request.Email,
           request.Password
       );
        var authResponse = new AuthenticationResponse
        (
            loginResult.Id,
            loginResult.FirstName,
            loginResult.LastName,
            loginResult.Email,
            loginResult.Token
        );

        return Ok(authResponse);
    }
}
