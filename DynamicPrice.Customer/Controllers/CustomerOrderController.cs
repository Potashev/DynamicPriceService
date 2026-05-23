using DynamicPrice.Customer.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class CustomerOrderController(
	ICoreApiClient coreApiClient) 
	: Controller
{
	[HttpPost]
	public async Task<IActionResult> Confirm(
		Guid cartId,
		CancellationToken cancellationToken)
	{
		var orderId = await coreApiClient.ConfirmOrder(cartId, cancellationToken);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	[HttpGet]
	public async Task<IActionResult> Details(
		Guid id,
		CancellationToken cancellationToken)
			=> View(await coreApiClient.OrderDetails(id, cancellationToken));

	[HttpPost]
	public async Task<IActionResult> Cancel(
		Guid orderId,
		CancellationToken cancellationToken)
	{
		await coreApiClient.CancelOrder(orderId, cancellationToken);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	[HttpGet]
	public async Task<IActionResult> GetReceiveKey(
		Guid orderId,
		int receiveKey)
	{
		ViewBag.OrderId = orderId;
		ViewBag.ReceiveKey = receiveKey;
		return View();
	}
}
