using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using DynamicPriceService.ViewModels;
using System.Text;
using System.Net.Http.Headers;

namespace DynamicPriceService.Controllers;
public class PriceRulesController : Controller
{
	private readonly string _localhosturl = "https://localhost:7140";
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly JsonSerializerOptions _options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	public PriceRulesController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task<IActionResult> Details()
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetStringAsync($"{_localhosturl}/api/company/price-rule");
		var priceRuleWithStatus = JsonSerializer.Deserialize<PriceRuleWithStatus>(response, _options);

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
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetStringAsync($"{_localhosturl}/api/company/price-rule");

		//to make api more compact, we use prVm with status
		var priceRuleWithStatus = JsonSerializer.Deserialize<PriceRuleWithStatus>(response, _options);
		return View(priceRuleWithStatus.PriceRuleVm);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, PriceRuleViewModel priceRuleVm)
	{
		if (ModelState.IsValid)
		{
			var client = _httpClientFactory.CreateClient();
			var token = HttpContext.Session.GetString("AuthToken");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			var json = JsonSerializer.Serialize(priceRuleVm);
			var data = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await client.PutAsync($"{_localhosturl}/api/company/price-rule", data);

			return RedirectToAction(nameof(Details));
		}

		return View(priceRuleVm);
	}

	public async Task<IActionResult> Run()
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.PostAsync($"{_localhosturl}/api/company/price-rule/run", null);
		return RedirectToAction(nameof(Details));
	}

	public async Task<IActionResult> Stop()
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.PostAsync($"{_localhosturl}/api/company/price-rule/stop", null);
		return RedirectToAction(nameof(Details));
	}
}
