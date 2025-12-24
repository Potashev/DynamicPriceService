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

	public async Task<ApplicationUser?> GetCurrentUserAsync()
		=> await _userManager.FindByIdAsync(UserId ?? string.Empty);

	public async Task<ApplicationUser> GetUserByIdAsync(string userId)
	=> await _userManager.FindByIdAsync(userId);

	public async Task<string> LoginUserAsync(string username, string password)
	{
		var user = await _userManager.FindByNameAsync(username);
		if (user == null || !await _userManager.CheckPasswordAsync(user, password))
			throw new ArgumentException("Unauthorized!");

		var roles = await _userManager.GetRolesAsync(user);
		var token = GenerateJwtToken(user, roles);

		//var expiresHours = int.TryParse(_config["Jwt:ExpireHours"], out var eh) ? eh : 1;

		//var cookieOptions = new CookieOptions
		//{
		//	HttpOnly = true,
		//	Secure = true,
		//	SameSite = SameSiteMode.Strict,
		//	Expires = DateTimeOffset.UtcNow.AddHours(expiresHours)
		//};

		//_httpContextAccessor.HttpContext.Response.Cookies.Append("DpAuth", token, cookieOptions);

		return token;
	}

	public async Task RegisterUserAsync(string username, string password, string email, string role)
	{
		//TODO: make better
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
		var user = await GetCurrentUserAsync();
		await _userManager.UpdateAsync(user);
	}

	public async Task UpdateUserAsync(ApplicationUser user)
		=> await _userManager.UpdateAsync(user);

	private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
	{
		//var claims = new List<Claim>
		//{
		//	//new(JwtRegisteredClaimNames.Sub, user.Id),
		//	//new(JwtRegisteredClaimNames.UniqueName, user.UserName)

		//	new(ClaimTypes.NameIdentifier, user.Id),
		//	new(ClaimTypes.Name, user.UserName)
		//};
		//claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, user.Id),
			new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
			//new(JwtRegisteredClaimNames.UniqueName, user.UserName),	// add email?

			//..roles.Select(r => new Claim(ClaimTypes.Role, r))
		};
		claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddMinutes(60),
			SigningCredentials = creds,
			Issuer = _config["Jwt:Issuer"],
			Audience = _config["Jwt:Audience"]
		};

		var tokenHandler = new JsonWebTokenHandler();
		string accessToken = tokenHandler.CreateToken(tokenDescriptor);
		return accessToken;

		//var expiresHours = int.TryParse(_config["Jwt:ExpireHours"], out var eh) ? eh : 1;

		//// use SecurityTokenDescriptor?
		//var token = new JwtSecurityToken(
		//	issuer: _config["Jwt:Issuer"],
		//	audience: _config["Jwt:Audience"],
		//	claims: claims,
		//	expires: DateTime.UtcNow.AddHours(expiresHours),
		//	signingCredentials: creds);

		////todo: check that token is valid

		//return new JwtSecurityTokenHandler().WriteToken(token);
	}
}

public interface IUserService
{
	string? UserId { get; }
	string? Role { get; }
	Task<ApplicationUser?> GetCurrentUserAsync();
	Task UpdateCurrentUserAsync();
	Task UpdateUserAsync(ApplicationUser user);
	Task<ApplicationUser> GetUserByIdAsync(string userId);
	Task RegisterUserAsync(string username, string password, string email, string role);
	Task<string> LoginUserAsync(string username, string password);
}
