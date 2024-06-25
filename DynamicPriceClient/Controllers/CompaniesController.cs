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
		var response = await client.GetStringAsync($"{_localhosturl}/api/ActiveCompanies");
		var activeCompanies = JsonSerializer.Deserialize<IEnumerable<Company>>(response, _options);	//todo: use dto
		return View(activeCompanies);
	}
}
