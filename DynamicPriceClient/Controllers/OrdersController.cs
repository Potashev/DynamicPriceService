using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace DynamicPriceClient.Controllers;
public class OrdersController : Controller
{
	private readonly string _localhosturl = "https://localhost:7140";
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly JsonSerializerOptions _options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	//todo: temp field to pass in mediator - remove later
	private readonly string _customerId = "1";

	public OrdersController(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task<IActionResult> CartOrderDetails(string companyId)
	{
		var client = _httpClientFactory.CreateClient();
		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
		var url = $"{_localhosturl}/api/Orders/Cart/{_customerId}/{companyId}";
		var response = await client.GetAsync(url, cts.Token);
		if (response.IsSuccessStatusCode)
		{
			var responseBody = await response.Content.ReadAsStringAsync();
			var cartOrder = JsonSerializer.Deserialize<Order>(responseBody, _options);
			return View(cartOrder);
		}
		else
		{
			return Content("The cart is empty");
		}
	}

	public async Task<IActionResult> AddProduct(int? id)
	{
		var client = _httpClientFactory.CreateClient();
		var url = $"{_localhosturl}/api/Orders/Add/{_customerId}/{id}";
		var response = await client.GetStringAsync(url);
		var cartOrder = JsonSerializer.Deserialize<Order>(response, _options);
		var companyId = cartOrder.Company.CompanyId;
		return RedirectToAction(nameof(CartOrderDetails), new { companyId });
	}

	public async Task<IActionResult> RemoveProduct(int? id)
	{
		var client = _httpClientFactory.CreateClient();
		var url = $"{_localhosturl}/api/Orders/Remove/{_customerId}/{id}";
		var response = await client.GetStringAsync(url);
		var cartOrder = JsonSerializer.Deserialize<Order>(response, _options);
		var companyId = cartOrder.Company.CompanyId;
		return RedirectToAction(nameof(CartOrderDetails), new { companyId });
	}

	public async Task<IActionResult> ConfirmOrder(int? id)
	{
		var client = _httpClientFactory.CreateClient();
		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
		var url = $"{_localhosturl}/api/Orders/Confirm/{_customerId}/{id}";
		var response = await client.GetStringAsync(url,cts.Token);
		var receiveKey = JsonSerializer.Deserialize<int>(response, _options);
		return Content($"Your receive Key: {receiveKey}");
	}

}
