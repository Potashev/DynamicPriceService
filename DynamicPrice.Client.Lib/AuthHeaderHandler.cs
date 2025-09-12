using System.Net.Http.Headers;

namespace DynamicPrice.Client.Common;

public class AuthHeaderHandler : DelegatingHandler
{
	private readonly IAuthTokenStore _authTokenStore;

	public AuthHeaderHandler(IAuthTokenStore authTokenStore)
	{
		_authTokenStore = authTokenStore ?? throw new ArgumentNullException(nameof(authTokenStore));
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var token = await _authTokenStore.GetToken();

		if (!string.IsNullOrEmpty(token))
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

		return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
	}
}
