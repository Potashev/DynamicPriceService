using DynamicPriceClient.Services;
using DynamicPriceClient.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DynamicPriceClient.Controllers;
public class CompaniesController : Controller
{
	private readonly HttpClientService _httpClientService;

	public CompaniesController(HttpClientService httpClientService)
    {
		_httpClientService = httpClientService;
	}

	public async Task<IActionResult> Index()
		=> View(await _httpClientService.GetAsync<IEnumerable<CompanyViewModel>>("api/companies?status=active"));

	public async Task<IActionResult> CompanyProducts(int? id)
		=> View(await _httpClientService.GetAsync<CompanyProductsInfo>($"api/companies/{id}/products"));
}
