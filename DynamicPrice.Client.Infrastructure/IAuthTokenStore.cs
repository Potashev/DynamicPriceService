using Microsoft.AspNetCore.Http;

namespace DynamicPrice.Client.Infrastructure;

public class SessionAuthTokenStore : IAuthTokenStore
{
	private readonly IHttpContextAccessor _contextAccessor;
	private const string KEY = "AuthToken";

	public SessionAuthTokenStore(IHttpContextAccessor contextAccessor)
		=> _contextAccessor = contextAccessor
		?? throw new ArgumentNullException(nameof(contextAccessor));

    public string GetToken()
	{
		var httpContext = _contextAccessor.HttpContext
			?? throw new ArgumentNullException(nameof(_contextAccessor.HttpContext));

		return httpContext.Session.GetString(KEY) 
			?? string.Empty;
	}

	public void SetToken(string authToken)
		=> _contextAccessor.HttpContext?.Session.SetString(KEY, authToken);
}

// Do not use when auth customer and manager together, since the KEY-cookie will be overwritten.
public class CookiesAuthTokenStore : IAuthTokenStore
{
	private readonly IHttpContextAccessor _contextAccessor;
	private const string KEY = "DpAuth";

	public CookiesAuthTokenStore(IHttpContextAccessor contextAccessor)
		=> _contextAccessor = contextAccessor
		?? throw new ArgumentNullException(nameof(contextAccessor));

	public string GetToken()
	{
		var httpContext = _contextAccessor.HttpContext
			?? throw new ArgumentNullException(nameof(_contextAccessor.HttpContext));

		return httpContext.Request.Cookies.TryGetValue(KEY, out var token)
			? token
			: string.Empty;
	}

	public void SetToken(string authToken)
		=> _contextAccessor.HttpContext?.Response.Cookies.Append(
			KEY,
			authToken,
			new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict });
}

public interface IAuthTokenStore
{
	string GetToken();
	void SetToken(string authToken);
}
