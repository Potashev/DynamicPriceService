using Microsoft.AspNetCore.Http;
using System.Text;

namespace DynamicPrice.Client.Infrastructure;

public class SessionAuthTokenStore : IAuthTokenStore
{
	private readonly IHttpContextAccessor _contextAccessor;

	public SessionAuthTokenStore(IHttpContextAccessor contextAccessor)
		=> _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));

	public async Task<string> GetToken()
		=> _contextAccessor.HttpContext.Session.GetString("AuthToken");

	public async Task SetToken(string authToken)
		=> _contextAccessor.HttpContext.Session.SetString("AuthToken", authToken);
}

public class CookiesAuthTokenStore : IAuthTokenStore
{
	private readonly IHttpContextAccessor _contextAccessor;
	private const string CookieName = "DpAuth";

	public CookiesAuthTokenStore(IHttpContextAccessor contextAccessor)
		=> _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));

	public async Task<string> GetToken()
		=> _contextAccessor.HttpContext.Request.Cookies.TryGetValue(CookieName, out var token) ? token : string.Empty;

	public async Task SetToken(string authToken)
		=> _contextAccessor.HttpContext.Response.Cookies.Append(CookieName, authToken, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict });
}

public interface IAuthTokenStore
{
	Task<string> GetToken();
	Task SetToken(string authToken);
}
