using DynamicPriceClient.ViewModels;
using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Mvc;
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
		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
		var response = await client.GetStringAsync($"{_localhosturl}/api/ActiveCompanies", cts.Token);
		var activeCompanies = JsonSerializer.Deserialize<IEnumerable<Company>>(response, _options);	//todo: use dto
		return View(activeCompanies);
	}

	public async Task<IActionResult> CompanyProducts(int? id)
	{
		var client = _httpClientFactory.CreateClient();
		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
		var response = await client.GetStringAsync($"{_localhosturl}/api/ActiveCompanies/{id}", cts.Token);
		var companyProductsInfo = JsonSerializer.Deserialize<CompanyProductsInfo>(response, _options);
		return View(companyProductsInfo);
	}
}
