using DynamicPriceService.ApiClients;
using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceService.Controllers;
public class AuthController : Controller
{
	private readonly ICoreApiClient _coreApiClient;

	public AuthController(ICoreApiClient coreApiClient)
		=> _coreApiClient = coreApiClient;

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
			HttpContext.Session.SetString("AuthToken", tokenResponse.Token);
			return RedirectToAction(nameof(Index), "Products");
		}
		return View();
	}
}
