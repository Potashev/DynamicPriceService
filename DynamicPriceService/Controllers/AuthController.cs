using DynamicPrice.Client.Common;
using DynamicPriceService.ApiClients;
using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceService.Controllers;
public class AuthController : Controller
{
	private readonly ICoreApiClient _coreApiClient;
	private readonly IAuthTokenStore _authTokenStore;

	public AuthController(IAuthTokenStore authTokenStore, ICoreApiClient coreApiClient)
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
	public async Task<IActionResult> Login(LoginViewModel loginVm)
	{
		if (ModelState.IsValid)
		{
			//todo: handle invalid login attempt
			// ModelState.AddModelError(string.Empty, "Invalid login attempt.");
			// return View(loginVm);

			var tokenResponse = await _coreApiClient.LoginManager(loginVm);

			_authTokenStore.SetToken(tokenResponse.Token);

			return RedirectToAction(nameof(Index), "Products");
		}
		return View();
	}
}
