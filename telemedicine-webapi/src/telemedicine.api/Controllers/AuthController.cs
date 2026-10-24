using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using telemedicine_webapi.Application.Common.Models;
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
    [AllowAnonymous]
    [EnableRateLimiting("authentication")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new BaseResponse { NotSuccesful = true }.SetError("Email and password are required"));
        }

        var result = await _authenticationService.Register(request, HttpContext.RequestAborted);
        return result.WasSuccesful ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("authentication")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new BaseResponse { NotSuccesful = true }.SetError("Email and password are required"));
        }

        var result = await _authenticationService.Login(request.Email, request.Password);
        return result.WasSuccesful ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("authentication")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return BadRequest(new BaseResponse { NotSuccesful = true }.SetError("Refresh token is required"));
        }

        return BadRequest(new BaseResponse { NotSuccesful = true }.SetError("Refresh token functionality not yet implemented"));
    }
}
