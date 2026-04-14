using DynamicPrice.Client.Infrastructure;
using DynamicPrice.Manager.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Manager.Controllers;

public class AuthController(
	ICoreApiClient coreApiClient,
	IAuthTokenStore authTokenStore) : Controller
{
	[HttpGet]
	public IActionResult LoginManager()
		=> View();

	[HttpPost]
	public async Task<IActionResult> LoginManager(
		LoginRequest loginVm,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
			return View(loginVm);

		var tokenResponse = await coreApiClient.LoginManager(loginVm, cancellationToken);

		authTokenStore.SetToken(tokenResponse.Token);

		return RedirectToAction(
			nameof(ProductsController.Index),
			nameof(ProductsController).Replace("Controller", ""));
	}

	[HttpPost]
	public async Task<IActionResult> Logout()
	{
		//await HttpContext.SignOutAsync("Cookies");

		authTokenStore.SetToken(string.Empty);

		return RedirectToAction(nameof(LoginManager));
	}
}