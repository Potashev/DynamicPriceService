using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DynamicPriceService.Services;

public class HttpClientService
{
	private readonly HttpClient _client;
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly JsonSerializerOptions _options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	public HttpClientService(HttpClient client, IHttpContextAccessor contextAccessor)
	{
		_client = client;
		_contextAccessor = contextAccessor;
	}

	private void AddAuthHeader()
	{
		var token = _contextAccessor.HttpContext.Session.GetString("AuthToken");
		if (!string.IsNullOrEmpty(token))
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
	}

	private CancellationToken GetCancellationToken(CancellationToken token)
	{
		if (!token.CanBeCanceled)
		{
			using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
			token = cts.Token;
		}
		return token;
	}

	public async Task<T?> GetAsync<T>(string url, CancellationToken token = default)
	{
		AddAuthHeader();

		var response = await _client.GetAsync(url, GetCancellationToken(token));
		response.EnsureSuccessStatusCode();

		var json = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<T>(json, _options);
	}

	public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body, CancellationToken token = default)
	{
		AddAuthHeader();

		var json = JsonSerializer.Serialize(body);
		using var content = new StringContent(json, Encoding.UTF8, "application/json");

		var response = await _client.PostAsync(url, content, GetCancellationToken(token));
		response.EnsureSuccessStatusCode();

		var responseJson = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<TResponse>(responseJson, _options);
	}

	public async Task PostAsync<TRequest>(
		string url,
		TRequest body,
		CancellationToken token = default)
	{
		AddAuthHeader();

		var json = JsonSerializer.Serialize(body);
		using var content = new StringContent(json, Encoding.UTF8, "application/json");

		var response = await _client.PostAsync(url, content, GetCancellationToken(token));
		response.EnsureSuccessStatusCode();
	}

	public async Task PostAsync(
	string url,
	CancellationToken token = default)
	{
		AddAuthHeader();

		var response = await _client.PostAsync(url, null, GetCancellationToken(token));
		response.EnsureSuccessStatusCode();
	}

	public async Task<TResponse?> PutAsync<TRequest, TResponse>(
		string url,
		TRequest data,
		CancellationToken token = default)
	{
		AddAuthHeader();

		var json = JsonSerializer.Serialize(data);
		var content = new StringContent(json, Encoding.UTF8, "application/json");

		var response = await _client.PutAsync(url, content, GetCancellationToken(token));
		response.EnsureSuccessStatusCode();

		var responseJson = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<TResponse>(responseJson, _options);
	}

	public async Task PutAsync<TRequest>(
		string url,
		TRequest data,
		CancellationToken token = default)
	{
		AddAuthHeader();

		var json = JsonSerializer.Serialize(data);
		var content = new StringContent(json, Encoding.UTF8, "application/json");

		var response = await _client.PutAsync(url, content, GetCancellationToken(token));
		response.EnsureSuccessStatusCode();
	}

	public async Task PatchAsync(
		string url,
		CancellationToken token = default)
	{
		AddAuthHeader();

		var response = await _client.PatchAsync(url, null, GetCancellationToken(token));
		response.EnsureSuccessStatusCode();
	}

	public async Task<TResponse?> DeleteAsync<TResponse>(
		string url, 
		CancellationToken token = default)
	{
		AddAuthHeader();

		var response = await _client.DeleteAsync(url, GetCancellationToken(token));
		response.EnsureSuccessStatusCode();

		var responseJson = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<TResponse>(responseJson, _options);
	}

	public async Task DeleteAsync(
	string url,
	CancellationToken token = default)
	{
		AddAuthHeader();

		var response = await _client.DeleteAsync(url, GetCancellationToken(token));
		response.EnsureSuccessStatusCode();

		//var responseJson = await response.Content.ReadAsStringAsync();
		//return JsonSerializer.Deserialize<TResponse>(responseJson, _options);
	}
}
