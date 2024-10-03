using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace DynamicPriceService.Controllers;
public class AccountController : Controller
{
	private readonly string _localhosturl = "https://localhost:7140";
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly JsonSerializerOptions _options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
	public ActionResult Login()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<ActionResult> Login(LoginViewModel loginVm)
	{
		if (ModelState.IsValid)
		{
			var client = _httpClientFactory.CreateClient();

			var json = JsonSerializer.Serialize(loginVm);
			var data = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await client.PostAsync($"{_localhosturl}/login?useCookies=true", data);
		}

		return View(loginVm);
	}
}
