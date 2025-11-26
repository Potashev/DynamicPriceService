using DynamicPrice.Manager.Controllers;
using DynamicPriceService.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceService.Controllers;
public class OrdersController : BaseController
{
	public OrdersController(ICoreApiClient coreApiClient)
		: base(coreApiClient) { }

	public async Task<IActionResult> Index()
	{
        //todo: handle 403
        var ordersVm = await CoreApiClient.GetOrders();
		return View(ordersVm);
	}

	public async Task<IActionResult> FindByReceiveKey(string key)
	{
		var orderId = await CoreApiClient.GetOrderIdByReceiveKey(key);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Details(int id)
	{
		var orderVm = await CoreApiClient.GetOrder(id);
		return View(orderVm);
	}

	public async Task<IActionResult> ReadyForReceive(string orderId)
	{
		await CoreApiClient.ReadyForReceiveOrder(orderId);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Complete(string orderId)
	{
		await CoreApiClient.CompleteOrder(orderId);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Statistics()
	{
		return View(await CoreApiClient.GetOrdersStatistics());
	} 
}
