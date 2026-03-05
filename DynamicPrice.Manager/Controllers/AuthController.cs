using DynamicPrice.Client.Infrastructure;
using DynamicPrice.Manager.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Manager.Controllers;

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

	public IActionResult Login()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(LoginRequest loginVm)
	{
		if (ModelState.IsValid)
		{
			//TODO: handle invalid login attempt
			// ModelState.AddModelError(string.Empty, "Invalid login attempt.");
			// return View(loginVm);

			var tokenResponse = await _coreApiClient.LoginManager(loginVm);
			await _authTokenStore.SetToken(tokenResponse.Token);

			return RedirectToAction(
				nameof(ProductsController.Index),
				nameof(ProductsController).Replace("Controller", ""));
		}
		return View();
	}
}
