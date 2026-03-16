using DynamicPrice.Customer.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class CompaniesController : BaseController
{
	public CompaniesController(ICoreApiClient coreApiClient)
		: base(coreApiClient) { }

	public async Task<IActionResult> Index(CancellationToken cancellationToken)
		=> View(await CoreApiClient.GetCompanies(cancellationToken));

	public async Task<IActionResult> CompanyProducts(
		int id,
		CancellationToken cancellationToken)
			=> View(await CoreApiClient.GetCompanyProducts(id, cancellationToken));
}
