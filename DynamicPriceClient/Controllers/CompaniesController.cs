using DynamicPrice.Customer.Controllers;
using DynamicPriceClient.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceClient.Controllers;
public class CompaniesController : BaseController
{
	public CompaniesController(ICoreApiClient coreApiClient)
		: base(coreApiClient) { }

	public async Task<IActionResult> Index()
		=> View(await CoreApiClient.GetCompanies());

	public async Task<IActionResult> CompanyProducts(int? id)
		=> View(await CoreApiClient.GetCompanyProducts((int)id));
}
