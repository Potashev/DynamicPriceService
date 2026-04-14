using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DynamicPrice.Client.Infrastructure;

public class CurrentUserService(
	IAuthTokenStore tokenStore)
{
	private JwtSecurityToken? GetToken()
	{
		var token = tokenStore.GetToken();
		if (string.IsNullOrEmpty(token)) return null;

		return new JwtSecurityTokenHandler().ReadJwtToken(token);
	}

	public string? UserId
		=> GetClaim(ClaimTypes.NameIdentifier);

	public string? CompanyTitle 
		=> GetClaim("company_title");

	public string? CustomerName
		=> GetClaim("customer_name");

	private string? GetClaim(string name)
		=> GetToken()?.Claims
			.FirstOrDefault(c => c.Type == name)?.Value;
}
