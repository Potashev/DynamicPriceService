using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DynamicPrice.Client.Infrastructure;

public class CurrentUserService(
	IAuthTokenStore tokenStore)
	: ICurrentUserService
{
	public string? UserId
		=> GetClaim(ClaimTypes.NameIdentifier);

	public string? UserName
		=> GetClaim(ClaimTypes.Name);

	public string? CompanyTitle 
		=> GetClaim("company_title");

	public bool IsAuthenticated
		=> GetToken() is not null;

	private string? GetClaim(string name)
		=> GetToken()?.Claims
			.FirstOrDefault(c => c.Type == name)?.Value;

	private JwtSecurityToken? GetToken()
	{
		var token = tokenStore.GetToken();

		if (string.IsNullOrEmpty(token))
			return null;

		return new JwtSecurityTokenHandler().ReadJwtToken(token);
	}
}

public interface ICurrentUserService
{
	string? UserId { get; }
	string? UserName { get; }
	string? CompanyTitle { get; }
	bool IsAuthenticated { get; }
}
