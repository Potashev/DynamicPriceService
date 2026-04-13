using DynamicPrice.Client.Infrastructure;
using DynamicPrice.Customer.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

		var tokenResponse = await _coreApiClient.LoginCustomer(loginVm, cancellationToken);

		_authTokenStore.SetToken(tokenResponse.Token);

		await AuthHelper.SignInWithJwtAsync(HttpContext, tokenResponse.Token);

		return RedirectToAction(
			nameof(CompaniesController.Index),
			nameof(CompaniesController).Replace("Controller", ""));
	}

	[HttpPost]
	public async Task<IActionResult> Logout()
	{
		// logout из MVC
		await HttpContext.SignOutAsync("Cookies");

		// очистка JWT
		_authTokenStore.SetToken(string.Empty);

		return RedirectToAction(nameof(LoginCustomer));
	}
}