using DynamicPrice.Manager.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Manager.Controllers;

public class CompanyOrdersController(
	ICoreApiClient сoreApiClient) : Controller
{

	[HttpGet]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
		=> View(await сoreApiClient.GetOrders(cancellationToken));

	[HttpGet]
	public async Task<IActionResult> FindByReceiveKey(
		string key,
		CancellationToken cancellationToken)
	{
		var orderId = await сoreApiClient.GetOrderIdByReceiveKey(key, cancellationToken);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	[HttpGet]
	public async Task<IActionResult> Details(
		int id,
		CancellationToken cancellationToken)
			=> View(await сoreApiClient.GetOrder(id, cancellationToken));

	[HttpPost]
	public async Task<IActionResult> ReadyForReceive(
		string orderId,
		CancellationToken cancellationToken)
	{
		await сoreApiClient.ReadyForReceiveOrder(orderId, cancellationToken);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	[HttpPost]
	public async Task<IActionResult> Complete(
		string orderId,
		CancellationToken cancellationToken)
	{
		await сoreApiClient.CompleteOrder(orderId, cancellationToken);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	[HttpGet]
	public async Task<IActionResult> Statistics(CancellationToken cancellationToken)
		=> View(await сoreApiClient.GetOrdersStatistics(cancellationToken));
}
