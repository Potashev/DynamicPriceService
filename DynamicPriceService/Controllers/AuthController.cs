using DynamicPriceService.Services;
using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceService.Controllers;
public class AuthController : Controller
{
	private readonly HttpClientService _httpClientService;

	public AuthController(HttpClientService httpClientService)
	{
		_httpClientService = httpClientService;
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
			var tokenResponse = await _httpClientService.PostAsync<LoginViewModel, TokenResponse>("api/auth/login", loginVm);
			HttpContext.Session.SetString("AuthToken", tokenResponse.Token);
			return RedirectToAction(nameof(Index), "Products");
		}
		return View();
	}
}
