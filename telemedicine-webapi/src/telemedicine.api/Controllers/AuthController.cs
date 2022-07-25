using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using telemedicine_webapi.Application.Common.Models.Authentication;
using telemedicine_webapi.Application.Services;

namespace telemedicine.api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authenticationService;

    public AuthController(IAuthService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var requestResult = await _authenticationService.Register(request.Email, request.Password, request.AccountType);

        return Ok(requestResult);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var loginResult = await _authenticationService.Login(request.Email, request.Password);

        return Ok(loginResult.Data);
    }
}
