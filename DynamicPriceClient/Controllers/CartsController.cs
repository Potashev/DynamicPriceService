using DynamicPriceClient.ViewModels;
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
		var url = $"{_localhosturl}/api/carts/{companyId}";
		var response = await client.GetAsync(url, cts.Token);
		if (response.IsSuccessStatusCode)
		{
			var responseBody = await response.Content.ReadAsStringAsync();
			var cart = JsonSerializer.Deserialize<CartViewModel>(responseBody, _options);
			return View(cart);
		}
		else
		{
			return Content("The cart is empty");
		}
	}

	public async Task<IActionResult> AddProduct(int? productId)
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var json = JsonSerializer.Serialize(productId);
		var data = new StringContent(json, Encoding.UTF8, "application/json");
		var response = await client.PostAsync($"{_localhosturl}/api/carts/items", data);
		var companyId = JsonSerializer.Deserialize<int>(await response.Content.ReadAsStringAsync());
		return RedirectToAction(nameof(CartDetails), new { companyId });
	}

	public async Task<IActionResult> RemoveProduct(int? productId)
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var response = await client.DeleteAsync($"{_localhosturl}/api/carts/items/{productId}");
		var companyId = JsonSerializer.Deserialize<int>(await response.Content.ReadAsStringAsync());
		return RedirectToAction(nameof(CartDetails), new { companyId });
	}

	public async Task<IActionResult> ConfirmOrder(int? cartId)
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
		var json = JsonSerializer.Serialize(cartId);
		var data = new StringContent(json, Encoding.UTF8, "application/json");
		var response = await client.PostAsync($"{_localhosturl}/api/orders", data, cts.Token);
		var receiveKey = JsonSerializer.Deserialize<int>(await response.Content.ReadAsStringAsync());
		return Content($"Your receive Key: {receiveKey}");
	}

}
