using DynamicPriceClient.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class OrderController : BaseController
{
	public OrderController(ICoreApiClient coreApiClient)
		: base(coreApiClient) { }

	public async Task<IActionResult> Confirm(int cartId)
	{
		var orderId = await CoreApiClient.ConfirmOrder(cartId);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Details(int id)
	{
		var orderVm = await CoreApiClient.OrderDetails(id);
		return View(orderVm);
	}

	public async Task<IActionResult> Cancel(int orderId)
	{
		await CoreApiClient.CancelOrder(orderId);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> GetReceiveKey(int orderId, int receiveKey)
	{
		ViewBag.OrderId = orderId;
		ViewBag.ReceiveKey = receiveKey;
		return View();
	}
}
