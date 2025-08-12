using DynamicPriceClient.ApiClients;
using DynamicPriceClient.Services;
using DynamicPriceClient.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceClient.Controllers;
public class CustomersController : Controller
{
	//private readonly HttpClientService _httpClientService;
	private readonly ICoreApiClient _coreApiClient;

	public CustomersController(ICoreApiClient coreApiClient)
	{
		_coreApiClient = coreApiClient;
	}

	//public async Task<IActionResult> GetCustomer()
	//	=> View(await _httpClientService.GetAsync<CustomerInfoViewModel>("api/customers/me"));

	public async Task<IActionResult> GetCustomer()
		=> View(await _coreApiClient.GetCustomer());


	[HttpPost, ActionName("TopUpBalance")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> TopUpBalance(string replenishmentAmount)
	{
		var balanceViewModel = new BalanceViewModel { ReplenishmentAmount = replenishmentAmount };
		//await _httpClientService.PutAsync("api/customers/me/balance", balanceViewModel);
		await _coreApiClient.TopUpBalance(balanceViewModel);
		return RedirectToAction(nameof(GetCustomer));
	}
}
