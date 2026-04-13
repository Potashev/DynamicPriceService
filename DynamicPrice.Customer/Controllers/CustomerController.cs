using DynamicPrice.Customer.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class CustomerController(
	ICoreApiClient coreApiClient) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
		=> View(await coreApiClient.GetCustomer(cancellationToken));

	[HttpPost]
	public async Task<IActionResult> TopUpBalance(
		BalanceRequest balanceRequest,
		CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			await coreApiClient.TopUpBalance(balanceRequest, cancellationToken);
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

		await coreApiClient.RegisterCustomer(registerVm, cancellationToken);

		return RedirectToAction(
				nameof(AuthController.LoginCustomer),
				nameof(AuthController).Replace("Controller", ""));
	}
}
