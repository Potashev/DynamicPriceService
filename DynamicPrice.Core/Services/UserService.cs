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
	private readonly IConfiguration _config;
	private readonly IMapper _mapper;
	private readonly DynamicPriceCoreContext _dpContext;

	public UserService(
		IHttpContextAccessor httpContextAccessor,
		UserManager<ApplicationUser> userManager,
		IConfiguration config,
		IMapper mapper,
		DynamicPriceCoreContext dpContext)
	{
		_httpContextAccessor = httpContextAccessor;
		_userManager = userManager;
		_config = config;
		_mapper = mapper;
		_dpContext = dpContext;
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

		var company = await GetCompanyAsync(user);

		return new TokenResponse
		{
			Token = GenerateJwtToken(user, roles, company)
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

	private async Task<Company?> GetCompanyAsync(ApplicationUser user)
	{
		if (!user.CompanyId.HasValue)
			return null;

		return await _dpContext.Companies
			.Where(c => c.CompanyId == user.CompanyId)
			.FirstOrDefaultAsync();
	}

	private string GenerateJwtToken(
		ApplicationUser user,
		IList<string> roles,
		Company company)
	{
		List<Claim> claims =
		[
			new(JwtRegisteredClaimNames.Sub, user.Id),
			new(ClaimTypes.NameIdentifier, user.Id),

			new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),

		//	new("company_id", user.CompanyId.ToString()),

		//			// добавляем только если есть
		//...(company is not null
		//	? [new Claim("company_title", company.Title)]
		//	: []),

			//new("company_title", GetCompanyTitle(user)),
			//new("company_title", user.Company.Title),

			..roles.Select(r => new Claim(ClaimTypes.Role, r))
		];

		if (roles.Contains("Customer"))
		{
			claims.Add(new("customer_name", user.UserName ?? ""));
		}

		if (roles.Contains("Manager") && company is not null)
		{
			claims.Add(new("company_id", company.CompanyId.ToString()));
			claims.Add(new("company_title", company.Title));
		}


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
	Task<IEnumerable<ApplicationUser>> GetUsersAsync(Expression<Func<ApplicationUser, bool>> predicate);
	Task RegisterUserAsync(RegisterUserRequest registerRequest);
	Task<TokenResponse> LoginUserAsync(LoginRequest loginRequest);
}
