using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DynamicPrice.Client.Infrastructure;

public class CurrentUserService
{
	private readonly IAuthTokenStore _tokenStore;

	public CurrentUserService(IAuthTokenStore tokenStore)
	{
		_tokenStore = tokenStore;
	}

	private JwtSecurityToken? GetToken()
	{
		var token = _tokenStore.GetToken();
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
	{
		return GetToken()?.Claims
			.FirstOrDefault(c => c.Type == name)?.Value;
	}
}
