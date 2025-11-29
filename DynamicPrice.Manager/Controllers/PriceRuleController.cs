using DynamicPriceService.ApiClients;
using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Manager.Controllers;

public class PriceRuleController : BaseController
{
	public PriceRuleController(ICoreApiClient coreApiClient)
		: base(coreApiClient) { }

	public async Task<IActionResult> Details()
	{
		var priceRuleWithStatus = await CoreApiClient.GetPriceRule();

		ViewData["RuleStatus"] = priceRuleWithStatus.IsActive ?
			"Running" :
			"Not running";

		return View(priceRuleWithStatus.PriceRuleVm);
	}

	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var priceRuleWithStatus = await CoreApiClient.GetPriceRule();

		return View(priceRuleWithStatus.PriceRuleVm);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, PriceRuleViewModel priceRuleVm)
	{
		if (ModelState.IsValid)
		{
			await CoreApiClient.UpdatePriceRule(priceRuleVm);
			return RedirectToAction(nameof(Details));
		}

		return View(priceRuleVm);
	}

	public async Task<IActionResult> Run()
	{
		await CoreApiClient.RunPriceReducing();
		return RedirectToAction(nameof(Details));
	}

	public async Task<IActionResult> Stop()
	{
		await CoreApiClient.StopPriceReducing();
		return RedirectToAction(nameof(Details));
	}
}
