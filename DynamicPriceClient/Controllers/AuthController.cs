using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace DynamicPriceClient.Controllers
{
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

		public IActionResult RegisterCustomer()
		{
			return View();
		}

		// POST: Products/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RegisterCustomer(RegisterViewModel registerVm)
		{
			registerVm.Role = "Customer";   //todo: is it right

			if (ModelState.IsValid)
			{
				var client = _httpClientFactory.CreateClient();
				var json = JsonSerializer.Serialize(registerVm);
				var data = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await client.PostAsync($"{_localhosturl}/api/Register", data);       //bad url?
				return RedirectToAction(nameof(Index));
			}
			return View();
		}
	}
}
