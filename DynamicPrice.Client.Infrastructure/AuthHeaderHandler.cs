using System.Net;
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

		try
		{
			return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (HttpRequestException ex)
		{
			var response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
			{
				RequestMessage = request,
				ReasonPhrase = "Network error",
				Content = new StringContent("Network error: " + ex.Message)
			};
			return response;
		}
		catch (Exception ex)
		{
			var response = new HttpResponseMessage(HttpStatusCode.BadGateway)
			{
				RequestMessage = request,
				ReasonPhrase = "Unexpected error",
				Content = new StringContent("Unexpected error: " + ex.Message)
			};
			return response;
		}
	}
}
