using DynamicPriceService.ApiClients;
using DynamicPriceService.Services;
using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceService.Controllers;
public class OrdersController : Controller
{
	//private readonly HttpClientService _httpClientService;
	private readonly ICoreApiClient _coreApiClient;

	public OrdersController(ICoreApiClient coreApiClient)
		=> _coreApiClient = coreApiClient;

	public async Task<IActionResult> Index()
	{
		//var ordersVm = await _httpClientService.GetAsync<IEnumerable<OrderViewModel>>("api/orders");
		var ordersVm = await _coreApiClient.GetOrders();
		return View(ordersVm);
	}

	public async Task<IActionResult> FindByReceiveKey(string key)
	{
		//var orderId = await _httpClientService.GetAsync<int>($"api/orders/by-receive-key/{key}");
		var orderId = await _coreApiClient.GetOrderIdByReceiveKey(key);

		//var orderVm = await _coreApiClient.GetOrderByReceiveKey(key);
		//return View(nameof(Details), orderVm);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Details(int id)
	{
		//var orderVm = await _httpClientService.GetAsync<OrderViewModel>($"api/orders/{id}");
		var orderVm = await _coreApiClient.GetOrder(id);
		return View(orderVm);
	}

	public async Task<IActionResult> CompleteOrder(string orderId)
	{
		//await _httpClientService.PatchAsync($"api/orders/{orderId}/complete");
		await _coreApiClient.CompleteOrder(orderId);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Statistics()
		=> View(await _coreApiClient.GetOrdersStatistics());
}
