using Microsoft.AspNetCore.Identity;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Application.Services;

namespace telemedicine_webapi.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IIdentityService _identityService;


    public AuthService(IJwtTokenGenerator jwtTokenGenerator, IIdentityService identityService)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _identityService=identityService;
    }

    public async Task<BaseResponse> Register(string email,string firstName,string lastName, string password, string accountType)
    {
        var existedUser=await _identityService.GetUserByEmailAsync(email);
        if(existedUser.Data != null)return OperationResult.NotSuccessful($"Account with email-{email} already exist");

        //var userAccountType=Enum.Parse<AccountType>(accountType, true);

        var createdUser=await _identityService.CreateUserAsync(email,password,accountType);
        var user=createdUser.Data as ApplicationUser;
        
        return OperationResult.Successful(user!);
    }

    public async Task<BaseResponse> Login(string email, string password)
    {
        var fetchedUser=await _identityService.GetUserByEmailAsync(email);
        if(fetchedUser.NotSuccesful)return OperationResult.NotSuccessful($"Account with email-{email} does not exist");
        
        var user=fetchedUser.Data as ApplicationUser;

        var verifyPassword=new PasswordHasher<ApplicationUser>().VerifyHashedPassword(user!,user?.PasswordHash,password);
        if(verifyPassword==PasswordVerificationResult.Failed)return OperationResult.NotSuccessful("Provided password is incorrect");

        var role=await _identityService.GetUserRoleAsync(user?.Email!);

        int userId=user?.Id??default(int);

        var token=_jwtTokenGenerator.GenerateJwtToken(userId, user?.Email!, role!);
        return OperationResult.Successful(token);
    }
}
