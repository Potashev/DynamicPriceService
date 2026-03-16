using DynamicPrice.Manager.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Manager.Controllers;

public class CompanyOrdersController : BaseController
{
	public CompanyOrdersController(ICoreApiClient coreApiClient)
		: base(coreApiClient) { }

	public async Task<IActionResult> Index(CancellationToken cancellationToken)
	{
		var ordersVm = await CoreApiClient.GetOrders(cancellationToken);
		return View(ordersVm);
	}

	public async Task<IActionResult> FindByReceiveKey(
		string key,
		CancellationToken cancellationToken)
	{
		var orderId = await CoreApiClient.GetOrderIdByReceiveKey(key, cancellationToken);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Details(
		int id,
		CancellationToken cancellationToken)
	{
		var orderVm = await CoreApiClient.GetOrder(id, cancellationToken);
		return View(orderVm);
	}

	public async Task<IActionResult> ReadyForReceive(
		string orderId,
		CancellationToken cancellationToken)
	{
		await CoreApiClient.ReadyForReceiveOrder(orderId, cancellationToken);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Complete(
		string orderId,
		CancellationToken cancellationToken)
	{
		await CoreApiClient.CompleteOrder(orderId, cancellationToken);
		return RedirectToAction(nameof(Details), new { id = orderId });
	}

	public async Task<IActionResult> Statistics(CancellationToken cancellationToken)
	{
		return View(await CoreApiClient.GetOrdersStatistics(cancellationToken));
	}
}
