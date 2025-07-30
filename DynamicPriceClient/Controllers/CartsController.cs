using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DynamicPriceClient.Controllers;
public class CartsController : Controller
{
	private readonly string _localhosturl = "https://localhost:7140";
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly JsonSerializerOptions _options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	public CartsController(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task<IActionResult> CartDetails(string companyId)
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
		var url = $"{_localhosturl}/api/Orders/Cart/{companyId}";
		var response = await client.GetAsync(url, cts.Token);
		if (response.IsSuccessStatusCode)
		{
			var responseBody = await response.Content.ReadAsStringAsync();
			var cart = JsonSerializer.Deserialize<Cart>(responseBody, _options);
			return View(cart);
		}
		else
		{
			return Content("The cart is empty");
		}
	}

	public async Task<IActionResult> AddProduct(int? id)
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var url = $"{_localhosturl}/api/Orders/Add/{id}";
		var response = await client.GetStringAsync(url);
		var cartOrder = JsonSerializer.Deserialize<Order>(response, _options);
		var companyId = cartOrder.Company.CompanyId;
		return RedirectToAction(nameof(CartDetails), new { companyId });
	}

	public async Task<IActionResult> RemoveProduct(int? id)
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var url = $"{_localhosturl}/api/Orders/Remove/{id}";
		var response = await client.GetStringAsync(url);
		var cartOrder = JsonSerializer.Deserialize<Order>(response, _options);
		var companyId = cartOrder.Company.CompanyId;
		return RedirectToAction(nameof(CartDetails), new { companyId });
	}

	public async Task<IActionResult> ConfirmOrder(int? id)
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
		var url = $"{_localhosturl}/api/Orders/Confirm/{id}";
		var response = await client.GetStringAsync(url,cts.Token);
		var receiveKey = JsonSerializer.Deserialize<int>(response, _options);
		return Content($"Your receive Key: {receiveKey}");
	}

}
