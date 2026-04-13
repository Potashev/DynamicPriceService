using DynamicPrice.Customer.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class CustomerController(ICoreApiClient CoreApiClient) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
		=> View(await CoreApiClient.GetCustomer(cancellationToken));

	[HttpPost]
	public async Task<IActionResult> TopUpBalance(
		BalanceRequest balanceRequest,
		CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			await CoreApiClient.TopUpBalance(balanceRequest, cancellationToken);
		}
		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public IActionResult RegisterCustomer()
		=> View();

	[HttpPost]
	public async Task<IActionResult> RegisterCustomer(
		RegisterRequest registerVm,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
			return View(registerVm);

		await CoreApiClient.RegisterCustomer(registerVm, cancellationToken);

		return RedirectToAction(
				nameof(AuthController.LoginCustomer),
				nameof(AuthController).Replace("Controller", ""));
	}
}
