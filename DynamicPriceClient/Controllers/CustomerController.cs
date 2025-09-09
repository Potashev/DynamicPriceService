using DynamicPrice.Customer.Controllers;
using DynamicPriceClient.ApiClients;
using DynamicPriceClient.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceClient.Controllers;
public class CustomerController : BaseController
{
	public CustomerController(ICoreApiClient coreApiClient)
		: base(coreApiClient) { }

	public async Task<IActionResult> Index()
		=> View(await CoreApiClient.GetCustomer());


	[HttpPost, ActionName("TopUpBalance")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> TopUpBalance(string replenishmentAmount)
	{
		var balanceViewModel = new BalanceViewModel { ReplenishmentAmount = replenishmentAmount };
		await CoreApiClient.TopUpBalance(balanceViewModel);
		return RedirectToAction(nameof(Index));
	}
}
