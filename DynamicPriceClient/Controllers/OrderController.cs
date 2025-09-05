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
		var orderId = await _coreApiClient.ConfirmOrder(cartId);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Details(int id)
	{
		var orderVm = await _coreApiClient.OrderDetails(id);
		return View(orderVm);
	}

	public async Task<IActionResult> Cancel(string orderId)
	{
		//todo: implement
		return Ok();
	}

	public async Task<IActionResult> GetReceiveKey(int orderId, int receiveKey)
	{
		ViewBag.OrderId = orderId;
		ViewBag.ReceiveKey = receiveKey;
		return View();
	}
}
