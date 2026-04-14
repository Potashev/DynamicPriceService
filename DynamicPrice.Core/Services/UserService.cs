using AutoMapper;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Shared.Contracts.Requests;
using DynamicPrice.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace DynamicPrice.Core.Services;

public class UserService(
		IHttpContextAccessor httpContextAccessor,
		UserManager<ApplicationUser> userManager,
		IMapper mapper,
		IUserCompanyService userCompanyService,
		ITokenService tokenService) : IUserService
{
	public string? UserId
		=> httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

	public string? Role
		=> httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

	public async Task<ApplicationUser> GetRequiredCurrentUserAsync()
		=> await GetUserByIdAsync(UserId ?? string.Empty)
			?? throw new UnauthorizedException("User is not authenticated.");

	public async Task<ApplicationUser> GetUserByIdAsync(string userId)
		=> await userManager.FindByIdAsync(userId)
			?? throw new NotFoundException("User not found.");

	public async Task<IEnumerable<ApplicationUser>> GetUsersAsync(Expression<Func<ApplicationUser, bool>> predicate)
		=> await userManager.Users
			.Where(predicate)
			.AsNoTracking()
			.ToListAsync();

	public async Task<TokenResponse> LoginUserAsync(LoginRequest request)
	{
		var user = await userManager.FindByNameAsync(request.Username);
		if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
			throw new UnauthorizedException("Unauthorized!");

		var roles = await userManager.GetRolesAsync(user);

		var company = await userCompanyService.GetCompanyAsync(user);

		return new TokenResponse
		{
			Token = tokenService.GenerateToken(user, roles, company)
		};
	}

	public async Task RegisterUserAsync(RegisterUserRequest request)
	{
		var user = mapper.Map<ApplicationUser>(request);

		var result = await userManager.CreateAsync(user, request.Password);
		if (!result.Succeeded)
		{
			throw new ApplicationException($"User creation failed!");
		}

		await userManager.AddToRoleAsync(user, request.Role);
	}

	public async Task UpdateCurrentUserAsync()
	{
		var user = await GetRequiredCurrentUserAsync();
		await userManager.UpdateAsync(user);
	}

	public async Task UpdateUserAsync(ApplicationUser user)
		=> await userManager.UpdateAsync(user);
}

public interface IUserService
{
	string? UserId { get; }
	string? Role { get; }
	Task<ApplicationUser> GetRequiredCurrentUserAsync();
	Task UpdateCurrentUserAsync();
	Task UpdateUserAsync(ApplicationUser user);
	Task<ApplicationUser> GetUserByIdAsync(string userId);
	Task<IEnumerable<ApplicationUser>> GetUsersAsync(Expression<Func<ApplicationUser, bool>> predicate);
	Task RegisterUserAsync(RegisterUserRequest registerRequest);
	Task<TokenResponse> LoginUserAsync(LoginRequest loginRequest);
}
