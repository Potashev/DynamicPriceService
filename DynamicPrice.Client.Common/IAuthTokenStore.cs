using Microsoft.AspNetCore.Http;
using System.Text;

namespace DynamicPrice.Client.Common;
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

public interface IAuthTokenStore
{
	Task<string> GetToken();
	Task SetToken(string authToken);
}
