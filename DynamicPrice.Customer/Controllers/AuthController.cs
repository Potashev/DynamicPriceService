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

		await _coreApiClient.RegisterCustomer(registerVm);
		return RedirectToAction(nameof(LoginCustomer));
	}

	public IActionResult LoginCustomer()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> LoginCustomer(LoginRequest loginVm)
	{
		if (!ModelState.IsValid)
			return View(loginVm);

		var tokenResponse = await _coreApiClient.LoginCustomer(loginVm);
		await _authTokenStore.SetToken(tokenResponse.Token);

		return RedirectToAction(
			nameof(CompaniesController.Index),
			nameof(CompaniesController).Replace("Controller", ""));
	}
}
