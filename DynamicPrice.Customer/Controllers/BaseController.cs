using DynamicPrice.Customer.ApiClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
		var customerName = "";
		try
		{
			var customer = await CoreApiClient.GetCustomer();
			customerName = customer.Name;
		}
		catch (Exception) { }

		ViewData["CustomerName"] = customerName;
		await next();
	}
}
