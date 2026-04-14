using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DynamicPrice.Client.Infrastructure;

public class CurrentUserService(
	IAuthTokenStore tokenStore) : ICurrentUserService
{
	public bool IsAuthenticated 
		=> GetToken() is not null;

	public string? UserId
		=> GetClaim(ClaimTypes.NameIdentifier);

	public string? CompanyTitle 
		=> GetClaim("company_title");

	public string? CustomerName
		=> GetClaim("customer_name");

	private string? GetClaim(string name)
		=> GetToken()?.Claims
			.FirstOrDefault(c => c.Type == name)?.Value;

	private JwtSecurityToken? GetToken()
	{
		var token = tokenStore.GetToken();
		if (string.IsNullOrEmpty(token)) return null;

		return new JwtSecurityTokenHandler().ReadJwtToken(token);
	}
}

public interface ICurrentUserService
{
	bool IsAuthenticated { get; }
	string? UserId { get; }
	string? CompanyTitle { get; }
	string? CustomerName { get; }
}
