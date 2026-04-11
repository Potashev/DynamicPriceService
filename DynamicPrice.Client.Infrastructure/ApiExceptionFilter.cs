namespace DynamicPrice.Customer.Extension;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refit;
using System.Text.Json;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;


public class ApiExceptionFilter(
	ILogger<ApiExceptionFilter> logger) : IAsyncExceptionFilter
{
	public async Task OnExceptionAsync(ExceptionContext context)
	{
		if (context.Exception is not ApiException apiEx)
			return;

		logger.LogWarning(apiEx, "API error");

		ProblemDetails? problem = null;

		try
		{
			problem = JsonSerializer.Deserialize<ProblemDetails>(apiEx.Content);
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to deserialize ProblemDetails");
		}

		context.ExceptionHandled = true;

		context.Result = CreateResult(context, apiEx, problem);
	}

	private IActionResult CreateResult(
		ExceptionContext context,
		ApiException ex,
		ProblemDetails? problem)
	{
		var message = ResolveMessage(ex, problem);

		var tempDataFactory = context.HttpContext.RequestServices
			.GetRequiredService<ITempDataDictionaryFactory>();

		var tempData = tempDataFactory.GetTempData(context.HttpContext);
		tempData["Error"] = message;

		var referer = context.HttpContext.Request.Headers["Referer"].ToString();

		if (!string.IsNullOrEmpty(referer))
			return new RedirectResult(referer);

		return new RedirectToActionResult("Index", "Home", null);
	}

	private static string ResolveMessage(ApiException ex, ProblemDetails? problem)
	{
		if (!string.IsNullOrWhiteSpace(problem?.Detail))
			return problem.Detail;

		return "Unable to reach server. Please try again later.";
	}
}
