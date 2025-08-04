using DynamicPriceService.Services;
using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceService.Controllers;
public class OrdersController : Controller
{
	private readonly HttpClientService _httpClientService;

	public OrdersController(HttpClientService httpClientService)
	{
		_httpClientService = httpClientService;
	}

	public async Task<IActionResult> Index()
	{
		var ordersVm = await _httpClientService.GetAsync<IEnumerable<OrderViewModel>>("api/orders");
		return View(ordersVm);
	}

	public async Task<IActionResult> FindByReceiveKey(string key)
	{
		var orderId = await _httpClientService.GetAsync<int>($"api/orders/by-receive-key/{key}");
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Details(int id)
	{
		var orderVm = await _httpClientService.GetAsync<OrderViewModel>($"api/orders/{id}");
		return View(orderVm);
	}

	public async Task<IActionResult> CompleteOrder(string orderId)
	{
		await _httpClientService.PatchAsync($"api/orders/{orderId}/complete");
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Statistics()
		=> View(await _httpClientService.GetAsync<OrderStatistics>("api/orders/statistics"));
}
