using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using DynamicPriceClient.ViewModels;

namespace DynamicPriceClient.Controllers;
public class CustomerController : Controller
{
	private readonly string _localhosturl = "https://localhost:7140";
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly JsonSerializerOptions _options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	public CustomerController(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task<IActionResult> GetCustomer()
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
		var response = await client.GetStringAsync($"{_localhosturl}/api/CustomerInfo", cts.Token);
		var customerInfo = JsonSerializer.Deserialize<CustomerInfoViewModel>(response, _options);
		return View(customerInfo);
	}
}
