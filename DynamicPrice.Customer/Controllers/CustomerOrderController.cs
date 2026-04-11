using DynamicPrice.Customer.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class CustomerOrderController : BaseController
{
	public CustomerOrderController(ICoreApiClient coreApiClient)
		: base(coreApiClient) { }

	[HttpPost]
	public async Task<IActionResult> Confirm(
		int cartId,
		CancellationToken cancellationToken)
	{
		var orderId = await CoreApiClient.ConfirmOrder(cartId, cancellationToken);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	[HttpGet]
	public async Task<IActionResult> Details(
		int id,
		CancellationToken cancellationToken)
			=> View(await CoreApiClient.OrderDetails(id, cancellationToken));

	[HttpPost]
	public async Task<IActionResult> Cancel(
		int orderId,
		CancellationToken cancellationToken)
	{
		await CoreApiClient.CancelOrder(orderId, cancellationToken);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	[HttpGet]
	public async Task<IActionResult> GetReceiveKey(
		int orderId,
		int receiveKey)
	{
		ViewBag.OrderId = orderId;
		ViewBag.ReceiveKey = receiveKey;
		return View();
	}
}
