using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DynamicPriceService.Controllers;
public class OrdersController : Controller
{
	private readonly string _localhosturl = "https://localhost:7140";
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly JsonSerializerOptions _options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	public OrdersController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task<IActionResult> Index()
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
		var response = await client.GetStringAsync($"{_localhosturl}/api/CompanyOrders", cts.Token);
		var ordersVm = JsonSerializer.Deserialize<IEnumerable<OrderViewModel>>(response, _options);
		return View(ordersVm);
	}

	public async Task<IActionResult> FindByReceiveKey(string key)
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetStringAsync($"{_localhosturl}/api/CompanyOrders/FindByReceiveKey/{key}");

		//todo: make better
		if (int.TryParse(response, out int id))
		{
			return RedirectToAction(nameof(Details), new { id });
		}
		return View(response);	// remove
	}

	public async Task<IActionResult> Details(int? id)
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetStringAsync($"{_localhosturl}/api/CompanyOrders/{id}");
		var orderVm = JsonSerializer.Deserialize<OrderViewModel>(response, _options);
		return View(orderVm);
	}

	public async Task<IActionResult> CompleteOrder(string orderId)
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetStringAsync($"{_localhosturl}/api/CompanyOrders/{orderId}/Complete");

		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Statistics()
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
		var response = await client.GetStringAsync($"{_localhosturl}/api/CompanyOrders/Statistics", cts.Token);
		var orderStatistics = JsonSerializer.Deserialize<OrderStatistics>(response, _options);
		return View(orderStatistics);
	}
}
