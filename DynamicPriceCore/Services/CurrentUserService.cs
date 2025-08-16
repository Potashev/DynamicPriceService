using Azure.Core;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DynamicPriceCore.Services;

public class CurrentUserService : ICurrentUserService
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IConfiguration _config;

	public CurrentUserService(
		IHttpContextAccessor httpContextAccessor,
		UserManager<ApplicationUser> userManager,
		IConfiguration config)
	{
		_httpContextAccessor = httpContextAccessor;
		_userManager = userManager;
		_config = config;
	}

	public string? UserId
		=> _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

	public string? Role 
		=> _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

	public async Task<ApplicationUser?> GetCurrentUserAsync()
		=> await _userManager.FindByIdAsync(UserId ?? string.Empty);

	public async Task<string> LoginUserAsync(string username, string password)
	{
		var user = await _userManager.FindByNameAsync(username);
		if (user == null || !await _userManager.CheckPasswordAsync(user, password))
			throw new ArgumentException("Unauthorized!");

		var roles = await _userManager.GetRolesAsync(user);
		var token = GenerateJwtToken(user, roles);
		return token;
	}

	public async Task RegisterUserAsync(string username, string password, string email, string role)
	{
		//todo: make better
		ApplicationUser user = new ApplicationUser
		{
			UserName = username,
			Email = email
		};

		switch (role)
		{
			case "Customer":
				user.Balance = 0;
				break;

			case "Manager":
				user.CompanyId = 1; //todo: replace with actual
				break;

			default:
				throw new ArgumentException("Invalid user role");
		}

		var result = await _userManager.CreateAsync(user, password);
		if (!result.Succeeded)
		{
			throw new ApplicationException($"User creation failed!");
		}

		await _userManager.AddToRoleAsync(user, role);
	}

	public async Task UpdateCurrentUserAsync()
	{
		var user = await GetCurrentUserAsync();
		await _userManager.UpdateAsync(user);
	}

	private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
	{
		var claims = new List<Claim>
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id),
			new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
		};

		claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		var token = new JwtSecurityToken(
			issuer: _config["Jwt:Issuer"],
			audience: _config["Jwt:Audience"],
			claims: claims,
			expires: DateTime.UtcNow.AddHours(1),
			signingCredentials: creds);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}

// rename to IUserService?
public interface ICurrentUserService
{
	string? UserId { get; }
	string? Role { get; }
	Task<ApplicationUser?> GetCurrentUserAsync();
	Task UpdateCurrentUserAsync();
	Task RegisterUserAsync(string username, string password, string email, string role);
	Task<string> LoginUserAsync(string username, string password);
}
