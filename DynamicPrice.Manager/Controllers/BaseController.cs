using DynamicPrice.Manager.ApiClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DynamicPrice.Manager.Controllers;

public class BaseController : Controller
{
	protected ICoreApiClient CoreApiClient { get; }

	public BaseController(ICoreApiClient coreApiClient)
	{
		CoreApiClient = coreApiClient;
	}

	public override async Task OnActionExecutionAsync(
		ActionExecutingContext context,
		ActionExecutionDelegate next)
	{
		var companyTitle = "";
		try
		{
			var company = await CoreApiClient.GetCompanyInfo();
			companyTitle = company.Title;
		}
		catch (Exception) { }

		ViewData["CompanyTitle"] = companyTitle;
		await next();
	}
}
