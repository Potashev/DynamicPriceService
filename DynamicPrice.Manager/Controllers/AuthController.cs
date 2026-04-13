using DynamicPrice.Client.Infrastructure;
using DynamicPrice.Manager.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

		var tokenResponse = await _coreApiClient.LoginManager(loginVm, cancellationToken);

		// ✅ сохраняем JWT (для API запросов)
		_authTokenStore.SetToken(tokenResponse.Token);

		// ✅ парсим JWT
		var handler = new JwtSecurityTokenHandler();
		var jwt = handler.ReadJwtToken(tokenResponse.Token);

		// ✅ создаём identity из claims токена
		var identity = new ClaimsIdentity(jwt.Claims, "Cookies");

		var principal = new ClaimsPrincipal(identity);

		// ✅ логиним пользователя в MVC (cookie auth)
		await HttpContext.SignInAsync("Cookies", principal);

		return RedirectToAction(
			nameof(ProductsController.Index),
			nameof(ProductsController).Replace("Controller", ""));
	}

	[HttpPost]
	public async Task<IActionResult> Logout()
	{
		// удаляем cookie авторизации
		await HttpContext.SignOutAsync("Cookies");

		// очищаем JWT
		_authTokenStore.SetToken(string.Empty);

		return RedirectToAction(nameof(LoginManager));
	}
}