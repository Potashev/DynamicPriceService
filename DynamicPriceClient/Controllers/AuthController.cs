using DynamicPriceClient.Services;
using DynamicPriceClient.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceClient.Controllers
{
	public class AuthController : Controller
	{
		private readonly HttpClientService _httpClientService;

		public AuthController(HttpClientService httpClientService)
		{
			_httpClientService = httpClientService;
		}

		public IActionResult RegisterCustomer()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RegisterCustomer(RegisterViewModel registerVm)
		{
			registerVm.Role = "Customer";   //todo: looks not good
			await _httpClientService.PostAsync("api/auth/register", registerVm);
			return RedirectToAction(nameof(Index));
		}

		public IActionResult LoginCustomer()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LoginCustomer(LoginViewModel loginVm)
		{

			if (ModelState.IsValid)
			{
				//todo: handle invalid login attempt
				// ModelState.AddModelError(string.Empty, "Invalid login attempt.");
				// return View(loginVm);
				var tokenResponse = await _httpClientService.PostAsync<LoginViewModel, TokenResponse>("api/auth/login", loginVm);
				HttpContext.Session.SetString("AuthToken", tokenResponse.Token);
				return RedirectToAction(nameof(Index), "Companies");
			}
			return View();
		}
	}
}
