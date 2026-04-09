using DynamicPrice.Manager.ApiClients;
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
	//[HttpPost]
	public async Task<IActionResult> FindByReceiveKey(
		string key,
		CancellationToken cancellationToken)
	{
		var orderId = await CoreApiClient.GetOrderIdByReceiveKey(key, cancellationToken);
		return RedirectToAction(nameof(Details), new { id = orderId });
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
