using DynamicPrice.Customer.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class CompaniesController(
	ICoreApiClient coreApiClient) 
	: Controller
{
	[HttpGet]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
		=> View(await coreApiClient.GetCompanies(cancellationToken));

	[HttpGet]
	public async Task<IActionResult> CompanyProducts(
		Guid id,
		CancellationToken cancellationToken)
			=> View(await coreApiClient.GetCompanyProducts(id, cancellationToken));
}
