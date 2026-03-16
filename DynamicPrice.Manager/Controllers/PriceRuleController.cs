using DynamicPrice.Manager.ApiClients;
using DynamicPrice.Shared.Contracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Manager.Controllers;

public class PriceRuleController : BaseController
{
	public PriceRuleController(ICoreApiClient coreApiClient)
		: base(coreApiClient) { }

	public async Task<IActionResult> Details(CancellationToken cancellationToken)
	{
		var priceRuleWithStatus = await CoreApiClient.GetPriceRule(cancellationToken);

		//todo: move logic to view
		ViewData["RuleStatus"] = priceRuleWithStatus.IsActive
			? "Running"
			: "Not running";

		return View(priceRuleWithStatus.PriceRule);
	}

	public async Task<IActionResult> Edit(
		int? id,
		CancellationToken cancellationToken)
	{
		if (id == null)
		{
			return NotFound();
		}

		var priceRuleWithStatus = await CoreApiClient.GetPriceRule(cancellationToken);

		return View(priceRuleWithStatus.PriceRule);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
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

	public async Task<IActionResult> Run(CancellationToken cancellationToken)
	{
		await CoreApiClient.RunPriceReducing(cancellationToken);
		return RedirectToAction(nameof(Details));
	}

	public async Task<IActionResult> Stop(CancellationToken cancellationToken)
	{
		await CoreApiClient.StopPriceReducing(cancellationToken);
		return RedirectToAction(nameof(Details));
	}
}
