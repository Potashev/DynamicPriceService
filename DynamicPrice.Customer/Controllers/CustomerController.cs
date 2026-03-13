using DynamicPrice.Customer.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
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
	public async Task<IActionResult> TopUpBalance(BalanceRequest balanceRequest)
	{
		await CoreApiClient.TopUpBalance(balanceRequest);
		return RedirectToAction(nameof(Index));
	}

	public IActionResult RegisterCustomer()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> RegisterCustomer(RegisterRequest registerVm)
	{
		if (!ModelState.IsValid)
			return View(registerVm);

		await CoreApiClient.RegisterCustomer(registerVm);

		return RedirectToAction(
				nameof(AuthController.LoginCustomer),
				nameof(AuthController).Replace("Controller", ""));
	}
}
