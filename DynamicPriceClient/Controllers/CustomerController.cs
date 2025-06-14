//using DynamicPriceCore.Models;
using DynamicPriceClient.ViewModels;
using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DynamicPriceClient.Controllers;
public class CustomerController : Controller
{
	private readonly string _localhosturl = "https://localhost:7140";
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly JsonSerializerOptions _options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	public CustomerController(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task<IActionResult> GetCustomer()
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
		var response = await client.GetStringAsync($"{_localhosturl}/api/Customer/Info", cts.Token);
		var customerInfo = JsonSerializer.Deserialize<CustomerInfoViewModel>(response, _options);
		return View(customerInfo);
	}

	[HttpPost, ActionName("TopUpBalance")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> TopUpBalance(string replenishmentAmount)
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

		var balanceViewModel = new BalanceViewModel { ReplenishmentAmount = replenishmentAmount };
		var json = JsonSerializer.Serialize(balanceViewModel);
		var data = new StringContent(json, Encoding.UTF8, "application/json");

		var response = await client.PutAsync($"{_localhosturl}/api/Customer/Balance", data);

		return RedirectToAction(nameof(GetCustomer));
	}

}
