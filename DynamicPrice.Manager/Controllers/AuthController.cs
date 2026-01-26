using DynamicPrice.Client.Infrastructure;
using DynamicPrice.Manager.ApiClients;
using DynamicPrice.Shared.Contracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Manager.Controllers;

//todo: try to remove from basecontroller - customer too
public class AuthController : BaseController
{
	private readonly IAuthTokenStore _authTokenStore;

	public AuthController(
		IAuthTokenStore authTokenStore,
		ICoreApiClient coreApiClient)
		: base(coreApiClient)
	{
		_authTokenStore = authTokenStore;
	}

	public IActionResult Login()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(LoginViewModel loginVm)
	{
		if (ModelState.IsValid)
		{
			//todo: handle invalid login attempt
			// ModelState.AddModelError(string.Empty, "Invalid login attempt.");
			// return View(loginVm);

			var tokenResponse = await CoreApiClient.LoginManager(loginVm);

			_authTokenStore.SetToken(tokenResponse.Token);

			return RedirectToAction(nameof(Index), "Products");
		}
		return View();
	}
}
