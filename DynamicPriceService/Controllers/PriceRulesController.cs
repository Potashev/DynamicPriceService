using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using DynamicPriceService.ViewModels;
using System.Text;

namespace DynamicPriceService.Controllers;
public class PriceRulesController : Controller
{
	//todo: temp field to pass in mediator - remove later
	private readonly string _userId;

	private readonly string _localhosturl = "https://localhost:7140";
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly JsonSerializerOptions _options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	public PriceRulesController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
	{
		_httpClientFactory = httpClientFactory;

		var context = httpContextAccessor.HttpContext;
		if (context.Request.Cookies.ContainsKey("User"))
			_userId = context.Request.Cookies["User"];
		else
			throw new Exception("User not found");
	}

	public async Task<IActionResult> Details()
	{
		var client = _httpClientFactory.CreateClient();
		//url looks not right
		var response = await client.GetStringAsync($"{_localhosturl}/api/{_userId}/PriceRule/Details");
		var priceRuleWithStatus = JsonSerializer.Deserialize<PriceRuleWithStatus>(response, _options);

		ViewData["RuleStatus"] = priceRuleWithStatus.IsActive ?
			"Running" :
			"Not running";

		return View(priceRuleWithStatus.PriceRuleVm);
	}

	// GET: priceRules/Edit/5
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}
		var client = _httpClientFactory.CreateClient();
		var response = await client.GetStringAsync($"{_localhosturl}/api/{_userId}/PriceRule/Details");

		//to make api more compact, we use prVm with status
		var priceRuleWithStatus = JsonSerializer.Deserialize<PriceRuleWithStatus>(response, _options);
		return View(priceRuleWithStatus.PriceRuleVm);
	}

	// POST: priceRules/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, PriceRuleViewModel priceRuleVm)
	{
		if (ModelState.IsValid)
		{
			var client = _httpClientFactory.CreateClient();

			var json = JsonSerializer.Serialize(priceRuleVm);
			var data = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await client.PutAsync($"{_localhosturl}/api/{_userId}/PriceRule/Edit", data);

			return RedirectToAction(nameof(Details));
		}

		return View(priceRuleVm);
	}

	public async Task<IActionResult> Run()
	{
		var client = _httpClientFactory.CreateClient();
		var response = await client.GetStringAsync($"{_localhosturl}/api/{_userId}/PriceRule/Run");
		return RedirectToAction(nameof(Details));
	}

	public async Task<IActionResult> Stop()
	{
		var client = _httpClientFactory.CreateClient();
		var response = await client.GetStringAsync($"{_localhosturl}/api/{_userId}/PriceRule/Stop");
		return RedirectToAction(nameof(Details));
	}
}
