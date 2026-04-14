using DynamicPrice.Core.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace DynamicPrice.Core.Services;

public class TokenService(
	IConfiguration config) : ITokenService
{
	public string GenerateToken(
		ApplicationUser user,
		IList<string> roles,
		Company? company)
	{
		List<Claim> claims =
		[
			new(JwtRegisteredClaimNames.Sub, user.Id),
			new(ClaimTypes.NameIdentifier, user.Id),
			new(ClaimTypes.Name, user.UserName ?? string.Empty),
			new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),

			..roles.Select(r => new Claim(ClaimTypes.Role, r))
		];

		//if (roles.Contains("Customer"))
		//{
		//	claims.Add(new("customer_name", user.UserName ?? ""));
		//}

		//if (roles.Contains("Manager") && company is not null)
		if (company is not null)
		{
			claims.Add(new("company_id", company.CompanyId.ToString()));
			claims.Add(new("company_title", company.Title));
		}

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var expireMinutes = config.GetValue<int>("Jwt:ExpireMinutes");

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
			SigningCredentials = credentials,
			Issuer = config["Jwt:Issuer"],
			Audience = config["Jwt:Audience"]
		};

		var tokenHandler = new JsonWebTokenHandler();
		string accessToken = tokenHandler.CreateToken(tokenDescriptor);
		return accessToken;
	}
}

public interface ITokenService
{
	string GenerateToken(ApplicationUser user, IList<string> roles, Company? company);
}