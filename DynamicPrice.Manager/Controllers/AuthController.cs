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

	public IActionResult LoginManager()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> LoginManager(LoginRequest loginVm)
	{
		if (ModelState.IsValid)
		{
			var tokenResponse = await _coreApiClient.LoginManager(loginVm);
			await _authTokenStore.SetToken(tokenResponse.Token);

			return RedirectToAction(
				nameof(ProductsController.Index),
				nameof(ProductsController).Replace("Controller", ""));
		}
		return View();
	}
}
