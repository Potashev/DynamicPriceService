using DynamicPrice.Client.Infrastructure;
using DynamicPrice.Customer.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class AuthController : Controller
{
	private readonly ICoreApiClient _coreApiClient;
	private readonly IAuthTokenStore _authTokenStore;

	public AuthController(
		IAuthTokenStore authTokenStore,
		ICoreApiClient coreApiClient)
	{
		_authTokenStore = authTokenStore;
		_coreApiClient = coreApiClient;
	}

	public IActionResult LoginCustomer()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> LoginCustomer(
		LoginRequest loginVm,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
			return View(loginVm);

		var tokenResponse = await _coreApiClient.LoginCustomer(loginVm, cancellationToken);
		await _authTokenStore.SetToken(tokenResponse.Token);

		return RedirectToAction(
			nameof(CompaniesController.Index),
			nameof(CompaniesController).Replace("Controller", ""));
	}
}
