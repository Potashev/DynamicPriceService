using DynamicPrice.Manager.ApiClients;
using DynamicPrice.Shared.Contracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Manager.Controllers;

public class PriceRuleController(ICoreApiClient CoreApiClient) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Details(CancellationToken cancellationToken)
		=> View(await CoreApiClient.GetPriceRule(cancellationToken));

	[HttpGet]
	public async Task<IActionResult> Edit(
		int? id,
		CancellationToken cancellationToken)
	{
		if (id is null)
			return NotFound();

		var priceRuleWithStatus = await CoreApiClient.GetPriceRule(cancellationToken);

		return View(priceRuleWithStatus.PriceRule);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(
		PriceRuleViewModel priceRuleVm,
		CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			await CoreApiClient.UpdatePriceRule(priceRuleVm, cancellationToken);
			return RedirectToAction(nameof(Details));
		}

		return View(priceRuleVm);
	}

	[HttpPost]
	public async Task<IActionResult> Run(CancellationToken cancellationToken)
	{
		await CoreApiClient.RunPriceReducing(cancellationToken);
		return RedirectToAction(nameof(Details));
	}

	[HttpPost]
	public async Task<IActionResult> Stop(CancellationToken cancellationToken)
	{
		await CoreApiClient.StopPriceReducing(cancellationToken);
		return RedirectToAction(nameof(Details));
	}
}
