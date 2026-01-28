using DynamicPrice.Customer.ApiClients;
using DynamicPrice.Shared.Contracts.ViewModels.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

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
		//todo: pass balancerequest from view instead of string
		var balanceViewModel = new BalanceRequest { ReplenishmentAmount = decimal.Parse(replenishmentAmount) };
		await CoreApiClient.TopUpBalance(balanceViewModel);
		return RedirectToAction(nameof(Index));
	}
}
