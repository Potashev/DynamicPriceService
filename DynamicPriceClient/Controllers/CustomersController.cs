using DynamicPriceClient.Services;
using DynamicPriceClient.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceClient.Controllers;
public class CustomersController : Controller
{
	private readonly HttpClientService _httpClientService;

	public CustomersController(HttpClientService httpClientService)
	{
		_httpClientService = httpClientService;
	}

	public async Task<IActionResult> GetCustomer()
		=> View(await _httpClientService.GetAsync<CustomerInfoViewModel>("api/customers/me"));


	[HttpPost, ActionName("TopUpBalance")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> TopUpBalance(string replenishmentAmount)
	{
		var balanceViewModel = new BalanceViewModel { ReplenishmentAmount = replenishmentAmount };
		await _httpClientService.PutAsync("api/customers/me/balance", balanceViewModel);
		return RedirectToAction(nameof(GetCustomer));
	}

}
