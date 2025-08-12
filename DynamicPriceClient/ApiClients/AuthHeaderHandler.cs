using System.Net.Http.Headers;

namespace DynamicPriceClient.ApiClients;

class AuthHeaderHandler : DelegatingHandler
{
	//private readonly IAuthTokenStore authTokenStore;
	private readonly IHttpContextAccessor _contextAccessor;

	public AuthHeaderHandler(/*IAuthTokenStore authTokenStore*/IHttpContextAccessor contextAccessor)
	{
		//this.authTokenStore = authTokenStore ?? throw new ArgumentNullException(nameof(authTokenStore));
		_contextAccessor = contextAccessor;

		// InnerHandler must be left as null when using DI, but must be assigned a value when
		// using RestService.For<IMyApi>
		// InnerHandler = new HttpClientHandler();
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		//var token = await authTokenStore.GetToken();

		//potentially refresh token here if it has expired etc.

		//request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		//request.Headers.Add("X-Tenant-Id", tenantProvider.GetTenantId());


		//todo: user abstraction IAuthTokenStore authTokenStore
		var token = _contextAccessor.HttpContext.Session.GetString("AuthToken");
		if (!string.IsNullOrEmpty(token))
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

		return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
	}
}
