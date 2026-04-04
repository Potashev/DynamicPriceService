using DynamicPrice.Customer.ApiClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Refit;

namespace DynamicPrice.Customer.Controllers;

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
			var customer = await CoreApiClient.GetCustomer();
			ViewData["CustomerName"] = customer.Name;
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
