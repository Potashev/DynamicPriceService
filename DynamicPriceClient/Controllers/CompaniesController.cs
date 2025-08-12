using DynamicPriceClient.ApiClients;
using DynamicPriceClient.Services;
using DynamicPriceClient.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DynamicPriceClient.Controllers;
public class CompaniesController : Controller
{
	private readonly ICoreApiClient _coreApiClient;

	public CompaniesController(ICoreApiClient coreApiClient)
    {
		_coreApiClient = coreApiClient;
	}

	public async Task<IActionResult> Index()
		=> View(await _coreApiClient.GetCompanies());

	//public async Task<IActionResult> CompanyProducts(int? id)
	//	=> View(await _httpClientService.GetAsync<CompanyProductsInfo>($"api/companies/{id}/products"));

	public async Task<IActionResult> CompanyProducts(int? id)
		=> View(await _coreApiClient.GetCompanyProducts((int)id));
}
