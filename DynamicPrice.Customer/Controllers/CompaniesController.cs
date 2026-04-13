using DynamicPrice.Customer.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class CompaniesController(ICoreApiClient CoreApiClient) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
		=> View(await CoreApiClient.GetCompanies(cancellationToken));

	[HttpGet]
	public async Task<IActionResult> CompanyProducts(
		int id,
		CancellationToken cancellationToken)
			=> View(await CoreApiClient.GetCompanyProducts(id, cancellationToken));
}
