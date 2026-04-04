using DynamicPrice.Manager.ApiClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Refit;

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
		try
		{
			var manager = await CoreApiClient.GetManager();
			ViewData["ManagerId"] = manager.Id;
			ViewData["CompanyTitle"] = manager.Company.Title;
		}
		catch (ApiException ex)
		{
			// можно игнорить или логировать
		}
		catch (Exception ex)
		{
			// логировать обязательно
		}

		await next();
	}
}
