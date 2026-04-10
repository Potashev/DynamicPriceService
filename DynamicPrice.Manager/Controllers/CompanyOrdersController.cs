using DynamicPrice.Manager.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Manager.Controllers;

public class CompanyOrdersController : BaseController
{
	public CompanyOrdersController(ICoreApiClient coreApiClient)
		: base(coreApiClient) { }

	[HttpGet]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
		=> View(await CoreApiClient.GetOrders(cancellationToken));

	[HttpGet]
	public async Task<IActionResult> FindByReceiveKey(
		ReceiveKeyRequest receiveKey,
		CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			var orderId = await CoreApiClient.GetOrderIdByReceiveKey(receiveKey.Key.ToString(), cancellationToken);
			return RedirectToAction(nameof(Details), new { id = orderId });
		}

		var orders = await CoreApiClient.GetOrders(cancellationToken);
		ViewData["FindModel"] = receiveKey;

		return View(nameof(Index), orders);

		//return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public async Task<IActionResult> Details(
		int id,
		CancellationToken cancellationToken)
			=> View(await CoreApiClient.GetOrder(id, cancellationToken));

	[HttpPost]
	public async Task<IActionResult> ReadyForReceive(
		string orderId,
		CancellationToken cancellationToken)
	{
		await CoreApiClient.ReadyForReceiveOrder(orderId, cancellationToken);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	[HttpPost]
	public async Task<IActionResult> Complete(
		string orderId,
		CancellationToken cancellationToken)
	{
		await CoreApiClient.CompleteOrder(orderId, cancellationToken);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	[HttpGet]
	public async Task<IActionResult> Statistics(CancellationToken cancellationToken)
		=> View(await CoreApiClient.GetOrdersStatistics(cancellationToken));
}
