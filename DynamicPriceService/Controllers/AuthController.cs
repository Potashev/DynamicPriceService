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

	// POST: Products/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

				// Сохранить токен (например, в куки или localStorage в браузере)
				HttpContext.Session.SetString("AuthToken", token);

				return RedirectToAction(nameof(Register));
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
				return View(loginVm);
			}

			return RedirectToAction(nameof(Index));
		}
		return View();
	}
}
