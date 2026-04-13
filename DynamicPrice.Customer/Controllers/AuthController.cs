using DynamicPrice.Client.Infrastructure;
using DynamicPrice.Customer.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DynamicPrice.Customer.Controllers;

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

		var tokenResponse = await _coreApiClient.LoginCustomer(loginVm, cancellationToken);

		//todo: moved to client.inf helper

		// ✅ сохраняем JWT (для API)
		_authTokenStore.SetToken(tokenResponse.Token);

		// ✅ парсим JWT
		var handler = new JwtSecurityTokenHandler();
		var jwt = handler.ReadJwtToken(tokenResponse.Token);

		// ✅ создаём identity из claims
		var identity = new ClaimsIdentity(jwt.Claims, "Cookies");
		var principal = new ClaimsPrincipal(identity);

		// ✅ логиним пользователя в MVC
		await HttpContext.SignInAsync("Cookies", principal);

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
		_authTokenStore.SetToken(string.Empty);

		return RedirectToAction(nameof(LoginCustomer));
	}
}