using DynamicPriceClient.ViewModels;
using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DynamicPriceClient.Controllers;
public class CompaniesController : Controller
{
	private readonly string _localhosturl = "https://localhost:7140";
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly JsonSerializerOptions _options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

    public CompaniesController(IHttpClientFactory httpClientFactory)
    {
			_httpClientFactory = httpClientFactory;
    }

	public async Task<IActionResult> Index()
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
		var response = await client.GetStringAsync($"{_localhosturl}/api/companies?status=active", cts.Token);
		var activeCompanies = JsonSerializer.Deserialize<IEnumerable<Company>>(response, _options);	//todo: use dto
		return View(activeCompanies);
	}

	public async Task<IActionResult> CompanyProducts(int? id)
	{
		var client = _httpClientFactory.CreateClient();
		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
		var response = await client.GetStringAsync($"{_localhosturl}/api/companies/{id}/products", cts.Token);
		var companyProductsInfo = JsonSerializer.Deserialize<CompanyProductsInfo>(response, _options);
		return View(companyProductsInfo);
	}
}
