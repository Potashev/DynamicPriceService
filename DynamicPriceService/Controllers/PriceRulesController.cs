using Microsoft.AspNetCore.Mvc;
using DynamicPriceService.ViewModels;
using DynamicPriceService.Services;

namespace DynamicPriceService.Controllers;
public class PriceRulesController : Controller
{
	private readonly HttpClientService _httpClientService;

	public PriceRulesController(HttpClientService httpClientService)
	{
		_httpClientService = httpClientService;
	}

	public async Task<IActionResult> Details()
	{
		var priceRuleWithStatus = await _httpClientService.GetAsync<PriceRuleWithStatus>("api/company/price-rule");

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

		var priceRuleWithStatus = await _httpClientService.GetAsync<PriceRuleWithStatus>("api/company/price-rule");

		return View(priceRuleWithStatus.PriceRuleVm);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, PriceRuleViewModel priceRuleVm)
	{
		if (ModelState.IsValid)
		{
			await _httpClientService.PutAsync("api/company/price-rule", priceRuleVm);
			return RedirectToAction(nameof(Details));
		}

		return View(priceRuleVm);
	}

	public async Task<IActionResult> Run()
	{
		await _httpClientService.PostAsync("api/company/price-rule/run");
		return RedirectToAction(nameof(Details));
	}

	public async Task<IActionResult> Stop()
	{
		await _httpClientService.PostAsync("api/company/price-rule/stop");
		return RedirectToAction(nameof(Details));
	}
}
