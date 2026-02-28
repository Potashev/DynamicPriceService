using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DynamicPrice.Core.Services;

public class UserService : IUserService
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IConfiguration _config;

	public UserService(
		IHttpContextAccessor httpContextAccessor,
		UserManager<ApplicationUser> userManager,
		IConfiguration config)
	{
		_httpContextAccessor = httpContextAccessor;
		_userManager = userManager;
		_config = config;
	}

	//TODO: use _userManager instead HttpContext?
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

	public async Task<string> LoginUserAsync(
		string username,
		string password)
	{
		var user = await _userManager.FindByNameAsync(username);
		if (user == null || !await _userManager.CheckPasswordAsync(user, password))
			throw new ArgumentException("Unauthorized!");

		var roles = await _userManager.GetRolesAsync(user);
		var token = GenerateJwtToken(user, roles);

		return token;
	}

	public async Task RegisterUserAsync(
		string username,
		string password,
		string email,
		string role)
	{
		//todo: make better
		ApplicationUser user = new()
		{
			UserName = username,
			Email = email,
			Balance = role switch
			{
				"Customer" => 0,
				_ => throw new ArgumentException("Invalid user role"),
			}
		};
		var result = await _userManager.CreateAsync(user, password);
		if (!result.Succeeded)
		{
			throw new ApplicationException($"User creation failed!");
		}

		await _userManager.AddToRoleAsync(user, role);
	}

	public async Task UpdateCurrentUserAsync()
	{
		var user = await GetRequiredCurrentUserAsync();
		await _userManager.UpdateAsync(user);
	}

	public async Task UpdateUserAsync(ApplicationUser user)
		=> await _userManager.UpdateAsync(user);

	private string GenerateJwtToken(
		ApplicationUser user,
		IList<string> roles)
	{
		List<Claim> claims =
		[
			new(JwtRegisteredClaimNames.Sub, user.Id),
			new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),

			..roles.Select(r => new Claim(ClaimTypes.Role, r))
		];

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var expireMinutes = _config.GetValue<int>("Jwt:ExpireMinutes");

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
			SigningCredentials = credentials,
			Issuer = _config["Jwt:Issuer"],
			Audience = _config["Jwt:Audience"]
		};

		var tokenHandler = new JsonWebTokenHandler();
		string accessToken = tokenHandler.CreateToken(tokenDescriptor);
		return accessToken;
	}
}

public interface IUserService
{
	string? UserId { get; }
	string? Role { get; }
	Task<ApplicationUser> GetRequiredCurrentUserAsync();
	Task UpdateCurrentUserAsync();
	Task UpdateUserAsync(ApplicationUser user);
	Task<ApplicationUser> GetUserByIdAsync(string userId);
	Task RegisterUserAsync(string username, string password, string email, string role);
	Task<string> LoginUserAsync(string username, string password);
}
