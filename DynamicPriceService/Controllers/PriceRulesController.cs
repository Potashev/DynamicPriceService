using DynamicPriceService.ApiClients;
using DynamicPriceService.Services;
using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceService.Controllers;
public class PriceRulesController : Controller
{
	//private readonly HttpClientService _httpClientService;
	private readonly ICoreApiClient _coreApiClient;

	public PriceRulesController(ICoreApiClient coreApiClient)
		=> _coreApiClient = coreApiClient;

	public async Task<IActionResult> Details()
	{
		//var priceRuleWithStatus = await _httpClientService.GetAsync<PriceRuleWithStatus>("api/company/price-rule");
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
			//await _httpClientService.PutAsync("api/company/price-rule", priceRuleVm);
			await _coreApiClient.UpdatePriceRule(priceRuleVm);
			return RedirectToAction(nameof(Details));
		}

		return View(priceRuleVm);
	}

	public async Task<IActionResult> Run()
	{
		//await _httpClientService.PostAsync("api/company/price-rule/run");
		await _coreApiClient.RunPriceReducing();
		return RedirectToAction(nameof(Details));
	}

	public async Task<IActionResult> Stop()
	{
		//await _httpClientService.PostAsync("api/company/price-rule/stop");
		await _coreApiClient.StopPriceReducing();
		return RedirectToAction(nameof(Details));
	}
}
