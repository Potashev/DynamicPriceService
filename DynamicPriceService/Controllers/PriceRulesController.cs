using DynamicPriceService.ApiClients;
using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceService.Controllers;
public class PriceRulesController : Controller
{
	private readonly ICoreApiClient _coreApiClient;

	public PriceRulesController(ICoreApiClient coreApiClient)
		=> _coreApiClient = coreApiClient;

	public async Task<IActionResult> Details()
	{
		var priceRuleWithStatus = await _coreApiClient.GetPriceRule();

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

		var priceRuleWithStatus = await _coreApiClient.GetPriceRule();

		return View(priceRuleWithStatus.PriceRuleVm);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, PriceRuleViewModel priceRuleVm)
	{
		if (ModelState.IsValid)
		{
			await _coreApiClient.UpdatePriceRule(priceRuleVm);
			return RedirectToAction(nameof(Details));
		}

		return View(priceRuleVm);
	}

	public async Task<IActionResult> Run()
	{
		await _coreApiClient.RunPriceReducing();
		return RedirectToAction(nameof(Details));
	}

	public async Task<IActionResult> Stop()
	{
		await _coreApiClient.StopPriceReducing();
		return RedirectToAction(nameof(Details));
	}
}
