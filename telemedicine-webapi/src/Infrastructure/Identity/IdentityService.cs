using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Domain.Enums;
using Microsoft.Extensions.Options;
using telemedicine_webapi.Infrastructure.JWTAuthentication;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IUserClaimsPrincipalFactory<IdentityUser<Guid>> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(UserManager<IdentityUser<Guid>> userManager, RoleManager<IdentityRole<Guid>> roleManager, IUserClaimsPrincipalFactory<IdentityUser<Guid>> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
    }

    public async Task<BaseResponse> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user !=null? OperationResult.Successful(user):OperationResult.NotSuccessful($"User with Id-{email} not found");
    }

    public async Task<string> GetUserNameAsync(Guid userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

        return user.UserName;
    }

    public async Task<BaseResponse> GetRoleAsync(string roleName)
    {
        var role=await _roleManager.FindByNameAsync(roleName);
        if(role != null)return OperationResult.Successful(role);
        return OperationResult.NotSuccessful("Not Found");
    }

    public async Task<string?> GetUserRoleAsync(string email)
    {
        var user=await _userManager.FindByEmailAsync(email);
        var roles=await _userManager.GetRolesAsync(user!);
        return roles?.FirstOrDefault();
    }

    public async Task<BaseResponse> CreateUserAsync(string email, string password, string roleName)
    {
        var result = new IdentityResult();
        var user=new IdentityUser<Guid>
        {
            Email=email,
            UserName=email
        };

        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null) return OperationResult.NotSuccessful("Role not Found");

        result = await _userManager.CreateAsync(user,password);
        if (!result.Succeeded) return OperationResult.NotSuccessful(String.Concat(result.Errors));

        result = await _userManager.AddToRoleAsync(user, role.ToString());

        return result.Succeeded?OperationResult.Successful(user):OperationResult.NotSuccessful("unable to create user");
    }

    public async Task<bool> IsInRoleAsync(Guid userId, string role)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(Guid userId, string policyName)
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

    public async Task<BaseResponse> DeleteUserAsync(Guid userId)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null ? await DeleteUserAsync(user) : OperationResult.NotSuccessful($"User with Id-{userId} not found");
    }

    private async Task<BaseResponse> DeleteUserAsync(IdentityUser<Guid> user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.Succeeded?OperationResult.Successful():OperationResult.NotSuccessful("Unable to delete user");
    }

    public async Task<BaseResponse<List<IdentityUser<Guid>>>> GetAllUsersAsync()
    {
        var result = _userManager.Users.ToList();

        return (BaseResponse<List<IdentityUser<Guid>>>)await Task.FromResult(OperationResult.Successful(result));
    }
}
