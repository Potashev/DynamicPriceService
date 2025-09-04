using DynamicPriceClient.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceClient.Controllers;
public class OrderController : Controller
{
	private readonly ICoreApiClient _coreApiClient;

	public OrderController(ICoreApiClient coreApiClient)
		=> _coreApiClient = coreApiClient;

	public async Task<IActionResult> Confirm(int cartId)
	{
		var receiveKey = await _coreApiClient.ConfirmOrder(cartId);
		return Content($"Your receive Key: {receiveKey}");
	}

	public async Task<IActionResult> Details(int id)
	{
		var orderVm = await _coreApiClient.OrderDetails(id);
		return View(orderVm);
	}

	public async Task<IActionResult> Cancel(string orderId)
	{
		return Ok();
		//await _coreApiClient.CompleteOrder(orderId);
		//return RedirectToAction(nameof(Details), new { id = orderId });
	}
}
