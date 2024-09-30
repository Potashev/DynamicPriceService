using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DynamicPriceService.Controllers;
public class OrdersController : Controller
{
	//todo: temp field to pass in mediator - remove later
	private readonly string _userId;

	private readonly string _localhosturl = "https://localhost:7140";
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly JsonSerializerOptions _options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	public OrdersController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
	{
		_httpClientFactory = httpClientFactory;

		var context = httpContextAccessor.HttpContext;
		if (context.Request.Cookies.ContainsKey("User"))
			_userId = context.Request.Cookies["User"];
		else
			throw new Exception("User not found");
	}

	public async Task<IActionResult> Index()
	{
		var client = _httpClientFactory.CreateClient();
		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
		var response = await client.GetStringAsync($"{_localhosturl}/api/{_userId}/CompanyOrders", cts.Token);
		var ordersVm = JsonSerializer.Deserialize<IEnumerable<OrderViewModel>>(response, _options);
		return View(ordersVm);
	}

	public async Task<IActionResult> FindByReceiveKey(string key)
	{
		var client = _httpClientFactory.CreateClient();
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
		var response = await client.GetStringAsync($"{_localhosturl}/api/CompanyOrders/{id}");
		var orderVm = JsonSerializer.Deserialize<OrderViewModel>(response, _options);
		return View(orderVm);
	}

	public async Task<IActionResult> CompleteOrder(string orderId)
	{
		var client = _httpClientFactory.CreateClient();
		var response = await client.GetStringAsync($"{_localhosturl}/api/CompanyOrders/Complete/{orderId}");

		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Statistics()
	{
		var client = _httpClientFactory.CreateClient();
		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
		var response = await client.GetStringAsync($"{_localhosturl}/api/{_userId}/CompanyOrders/Statistics", cts.Token);
		var orderStatistics = JsonSerializer.Deserialize<OrderStatistics>(response, _options);
		return View(orderStatistics);
	}
}
