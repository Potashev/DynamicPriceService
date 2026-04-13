using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Shared.Contracts.Requests;
using DynamicPrice.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace DynamicPrice.Core.Services;

public class UserService : IUserService
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IMapper _mapper;
	private readonly IUserCompanyService _userCompanyService;
	private readonly ITokenService _tokenService;

	public UserService(
		IHttpContextAccessor httpContextAccessor,
		UserManager<ApplicationUser> userManager,
		IMapper mapper,
		IUserCompanyService userCompanyService,
		ITokenService tokenService)
	{
		_httpContextAccessor = httpContextAccessor;
		_userManager = userManager;
		_mapper = mapper;
		_userCompanyService = userCompanyService;
		_tokenService = tokenService;
	}

	public string? UserId
		=> _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

	public string? Role
		=> _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

	public async Task<ApplicationUser> GetRequiredCurrentUserAsync()
		=> await GetUserByIdAsync(UserId ?? string.Empty)
			?? throw new UnauthorizedException("User is not authenticated.");

	public async Task<ApplicationUser> GetUserByIdAsync(string userId)
		=> await _userManager.FindByIdAsync(userId)
			?? throw new NotFoundException("User not found.");

	public async Task<IEnumerable<ApplicationUser>> GetUsersAsync(Expression<Func<ApplicationUser, bool>> predicate)
		=> await _userManager.Users
			.Where(predicate)
			.AsNoTracking()
			.ToListAsync();

	public async Task<TokenResponse> LoginUserAsync(LoginRequest request)
	{
		var user = await _userManager.FindByNameAsync(request.Username);
		if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
			throw new UnauthorizedException("Unauthorized!");

		var roles = await _userManager.GetRolesAsync(user);

		var company = await _userCompanyService.GetCompanyAsync(user);

		return new TokenResponse
		{
			Token = _tokenService.GenerateToken(user, roles, company)
		};
	}

	public async Task RegisterUserAsync(RegisterUserRequest request)
	{
		var user = _mapper.Map<ApplicationUser>(request);

		var result = await _userManager.CreateAsync(user, request.Password);
		if (!result.Succeeded)
		{
			throw new ApplicationException($"User creation failed!");
		}

		await _userManager.AddToRoleAsync(user, request.Role);
	}

	public async Task UpdateCurrentUserAsync()
	{
		var user = await GetRequiredCurrentUserAsync();
		await _userManager.UpdateAsync(user);
	}

	public async Task UpdateUserAsync(ApplicationUser user)
		=> await _userManager.UpdateAsync(user);
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
