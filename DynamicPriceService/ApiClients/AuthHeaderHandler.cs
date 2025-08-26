using System.Net.Http.Headers;

namespace DynamicPriceService.ApiClients;

class AuthHeaderHandler : DelegatingHandler
{
	//private readonly IAuthTokenStore authTokenStore;
	private readonly IHttpContextAccessor _contextAccessor;

	public AuthHeaderHandler(/*IAuthTokenStore authTokenStore*/IHttpContextAccessor contextAccessor)
	{
		//this.authTokenStore = authTokenStore ?? throw new ArgumentNullException(nameof(authTokenStore));
		_contextAccessor = contextAccessor;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		//var token = await authTokenStore.GetToken();

		var token = _contextAccessor.HttpContext.Session.GetString("AuthToken");
		if (!string.IsNullOrEmpty(token))
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

		return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
	}
}
