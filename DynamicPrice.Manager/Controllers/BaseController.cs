using DynamicPrice.Manager.ApiClients;
using DynamicPrice.Manager.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

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

			var executedContext = await next();

			if (executedContext.Exception is Refit.ApiException apiEx)
			{
				executedContext.ExceptionHandled = true;
				context.Result = await HandleApiException(apiEx);
			}
		}
		catch (Refit.ApiException ex)
		{
			context.Result = await HandleApiException(ex);
		}
		catch (Exception)
		{
			context.Result = View("Error");
		}
	}

	private async Task<IActionResult> HandleApiException(Refit.ApiException ex)
	{
		var problem = await ex.GetProblemDetailsAsync();

		return ex.StatusCode switch
		{
			HttpStatusCode.BadRequest => HandleBadRequest(problem),
			HttpStatusCode.NotFound => NotFound(),
			HttpStatusCode.Unauthorized => RedirectToAction("Login", "Auth"),
			_ => View("Error", problem)
		};
	}

	private IActionResult HandleBadRequest(ProblemDetails? problem)
	{
		if (problem is ValidationProblemDetails validation)
		{
			foreach (var (key, errors) in validation.Errors)
			{
				foreach (var error in errors)
				{
					ModelState.AddModelError(key, error);
				}
			}
		}
		else if (problem != null)
		{
			ModelState.AddModelError("", problem.Detail);
		}

		//// ⚠️ вот тут нюанс (см. ниже)
		//return View();

		var actionName = ControllerContext.ActionDescriptor.ActionName;
		return View(actionName);
	}
}
