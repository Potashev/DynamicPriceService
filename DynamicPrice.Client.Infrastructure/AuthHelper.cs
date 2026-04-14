using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DynamicPrice.Client.Infrastructure;

public static class AuthHelper
{
	public static async Task SignInWithJwtAsync(
		HttpContext context, 
		string jwtToken,
		string cookiesName)
	{
		var handler = new JwtSecurityTokenHandler();
		var jwt = handler.ReadJwtToken(jwtToken);

		var identity = new ClaimsIdentity(jwt.Claims, cookiesName);
		var principal = new ClaimsPrincipal(identity);

		await context.SignInAsync(cookiesName, principal);
	}
}
