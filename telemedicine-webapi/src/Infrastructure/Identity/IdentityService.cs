using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Domain.Enums;
using Microsoft.Extensions.Options;
using telemedicine_webapi.Infrastructure.JWTAuthentication;

namespace telemedicine_webapi.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
    }

    public async Task<BaseResponse> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user !=null? OperationResult.Successful(user):OperationResult.NotSuccessful($"User with Id-{email} not found");
    }

    public async Task<string> GetUserNameAsync(int userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

        return user.UserName;
    }

    public async Task<string?> GetUserRoleAsync(string email)
    {
        var user=await _userManager.FindByEmailAsync(email);
        var roles=await _userManager.GetRolesAsync(user!);
        return roles?.FirstOrDefault();
    }

    public async Task<BaseResponse> CreateUserAsync(string email, string password, string role)
    {
        var user=new ApplicationUser
        {
            Email=email,
            UserName=email,
        };

        await _userManager.AddToRoleAsync(user, role);

        var result = await _userManager.CreateAsync(user,password);
        
        return result.Succeeded?OperationResult.Successful(user):OperationResult.NotSuccessful("unable to create user");
    }

    public async Task<bool> IsInRoleAsync(int userId, string role)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(int userId, string policyName)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<BaseResponse> DeleteUserAsync(int userId)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null ? await DeleteUserAsync(user) : OperationResult.NotSuccessful($"User with Id-{userId} not found");
    }

    private async Task<BaseResponse> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.Succeeded?OperationResult.Successful():OperationResult.NotSuccessful("Unable to delete user");
    }
}
