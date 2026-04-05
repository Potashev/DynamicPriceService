namespace DynamicPrice.Customer.Extension;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Refit;
using System.Text.Json;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;


public class ApiExceptionFilter : IAsyncExceptionFilter
{
	private readonly ILogger<ApiExceptionFilter> _logger;

	public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
	{
		_logger = logger;
	}

	public async Task OnExceptionAsync(ExceptionContext context)
	{
		if (context.Exception is not ApiException apiEx)
			return;

		_logger.LogWarning(apiEx, "API error");

		ProblemDetails? problem = null;

		try
		{
			problem = JsonSerializer.Deserialize<ProblemDetails>(apiEx.Content);
		}
		catch { }

		context.ExceptionHandled = true;

		context.Result = CreateResult(context, apiEx, problem);
	}

	private IActionResult CreateResult(
		ExceptionContext context,
		ApiException ex,
		ProblemDetails? problem)
	{
		switch ((int)ex.StatusCode)
		{
			case 400:
			case 401:
			case 403:
			case 404:
			case 409:
				return CreateViewResult(context, problem?.Detail ?? "Request failed");
			default:
				return CreateViewResult(context, "Something went wrong");
		}
	}

	private ViewResult CreateViewResult(ExceptionContext context, string errorMessage)
	{
		var viewData = new ViewDataDictionary(
			new EmptyModelMetadataProvider(),
			context.ModelState);

		viewData.ModelState.AddModelError("", errorMessage);

		return new ViewResult
		{
			ViewData = viewData
		};
	}
}
