using DynamicPrice.Client.Infrastructure;
using DynamicPrice.Customer.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class AuthController(
	ICoreApiClient coreApiClient,
	IAuthTokenStore authTokenStore) 
	: Controller
{
	[HttpGet]
	public IActionResult LoginCustomer()
		=> View();

	[HttpPost]
	public async Task<IActionResult> LoginCustomer(
		LoginRequest loginVm,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
			return View(loginVm);

		var tokenResponse = await coreApiClient.LoginCustomer(loginVm, cancellationToken);

		authTokenStore.SetToken(tokenResponse.Token);

		return RedirectToAction(
			nameof(CompaniesController.Index),
			nameof(CompaniesController).Replace("Controller", ""));
	}

	[HttpPost]
	public async Task<IActionResult> Logout()
	{
		authTokenStore.SetToken(string.Empty);

		return RedirectToAction(nameof(LoginCustomer));
	}
}