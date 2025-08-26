using DynamicPriceClient.ApiClients;
using Microsoft.AspNetCore.Mvc;

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

	public async Task<IActionResult> CompanyProducts(int? id)
		=> View(await _coreApiClient.GetCompanyProducts((int)id));
}
