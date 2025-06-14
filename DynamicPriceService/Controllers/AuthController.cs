using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace DynamicPriceService.Controllers;
public class AuthController : Controller
{
	private readonly string _localhosturl = "https://localhost:7140";
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly JsonSerializerOptions _options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	public AuthController(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Register()
	{
		return View();
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
			var client = _httpClientFactory.CreateClient();
			var json = JsonSerializer.Serialize(loginVm);
			var data = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await client.PostAsync($"{_localhosturl}/api/Login", data);       //bad url?

			if (response.IsSuccessStatusCode)
			{
				var responseContent = await response.Content.ReadAsStringAsync();

				//todo: compare with client and remove token respose in client
				var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);
				string token = dict["token"];

				HttpContext.Session.SetString("AuthToken", token);

				return RedirectToAction(nameof(Index), "Products");
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
				return View(loginVm);
			}
		}
		return View();
	}
}
