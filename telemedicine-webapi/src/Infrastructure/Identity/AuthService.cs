using Microsoft.AspNetCore.Identity;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Application.Services;
using telemedicine_webapi.Infrastructure.Persistence.Context;

namespace telemedicine_webapi.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public AuthService(IJwtTokenGenerator jwtTokenGenerator, IIdentityService identityService, IApplicationDbContext context)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _identityService=identityService;
        _context = context;
    }

    public async Task<BaseResponse> Register(string email,string password, string accountType)
    {
        var existedUser=await _identityService.GetUserByEmailAsync(email);
        if(existedUser.Data != null)return OperationResult.NotSuccessful($"Account with email-{email} already exist");

        var createUserOperation=await _identityService.CreateUserAsync(email,password,accountType);
        if (createUserOperation.NotSuccesful) return OperationResult.NotSuccessful(createUserOperation.Errors);
        var user = createUserOperation.Data as IdentityUser<Guid>;

        var result = await _context.SaveChangesAsync();
        
        return OperationResult.Successful();
    }

    public async Task<BaseResponse> Login(string email, string password)
    {
        var fetchedUser = await _identityService.GetUserByEmailAsync(email);
        if (fetchedUser.NotSuccesful) return OperationResult.NotSuccessful($"Account with email-{email} does not exist");

        var user = fetchedUser.Data as IdentityUser<Guid>;

        var verifyPassword=new PasswordHasher<IdentityUser<Guid>>().VerifyHashedPassword(user!,user?.PasswordHash,password);
        if(verifyPassword==PasswordVerificationResult.Failed)return OperationResult.NotSuccessful("Provided password is incorrect");

        var role = await _identityService.GetUserRoleAsync(user?.Email!);

        var userId = user?.Id ?? default(Guid);

        var token = _jwtTokenGenerator.GenerateJwtToken(userId, user?.Email!, role!);
        return OperationResult.Successful(token);
    }
}
