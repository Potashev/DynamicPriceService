using DynamicPrice.Client.Common;
using DynamicPrice.Customer.Controllers;
using DynamicPriceClient.ApiClients;
using DynamicPriceClient.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceClient.Controllers
{
	public class AuthController : BaseController
	{
		private readonly IAuthTokenStore _authTokenStore;

		public AuthController(IAuthTokenStore authTokenStore, ICoreApiClient coreApiClient)
			: base(coreApiClient)
		{
			_authTokenStore = authTokenStore;
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
			await CoreApiClient.RegisterCustomer(registerVm);
			return RedirectToAction(nameof(LoginCustomer));
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

				var tokenResponse = await CoreApiClient.LoginCustomer(loginVm);
				_authTokenStore.SetToken(tokenResponse.Token);
				return RedirectToAction(nameof(Index), "Companies");
			}
			return View();
		}
	}
}
