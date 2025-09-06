using DynamicPriceService.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceService.Controllers;
public class OrdersController : Controller
{
	private readonly ICoreApiClient _coreApiClient;

	public OrdersController(ICoreApiClient coreApiClient)
		=> _coreApiClient = coreApiClient;

	public async Task<IActionResult> Index()
	{
		var ordersVm = await _coreApiClient.GetOrders();
		return View(ordersVm);
	}

	public async Task<IActionResult> FindByReceiveKey(string key)
	{
		var orderId = await _coreApiClient.GetOrderIdByReceiveKey(key);

		//var orderVm = await _coreApiClient.GetOrderByReceiveKey(key);
		//return View(nameof(Details), orderVm);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Details(int id)
	{
		var orderVm = await _coreApiClient.GetOrder(id);
		return View(orderVm);
	}

	public async Task<IActionResult> ReadyForReceive(string orderId)
	{
		await _coreApiClient.ReadyForReceiveOrder(orderId);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Complete(string orderId)
	{
		await _coreApiClient.CompleteOrder(orderId);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Statistics()
	{
		return View(await _coreApiClient.GetOrdersStatistics());
	} 
}
