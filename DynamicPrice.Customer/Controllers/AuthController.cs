using DynamicPrice.Client.Infrastructure;
using DynamicPrice.Customer.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class AuthController(
	ICoreApiClient coreApiClient,
	IAuthTokenStore authTokenStore) : Controller
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

		await AuthHelper.SignInWithJwtAsync(HttpContext, tokenResponse.Token, "CustomerCookies");

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
		authTokenStore.SetToken(string.Empty);

		return RedirectToAction(nameof(LoginCustomer));
	}
}