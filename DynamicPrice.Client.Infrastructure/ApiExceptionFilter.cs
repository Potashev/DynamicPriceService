namespace DynamicPrice.Customer.Extension;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
		var message = problem?.Detail ?? "Request failed";

		if (HttpMethods.IsGet(context.HttpContext.Request.Method))
		{
			return CreateViewResult(context, message);
		}

		var tempDataFactory = context.HttpContext.RequestServices
			.GetRequiredService<ITempDataDictionaryFactory>();

		var tempData = tempDataFactory.GetTempData(context.HttpContext);
		tempData["Error"] = message;

		var referer = context.HttpContext.Request.Headers["Referer"].ToString();

		if (!string.IsNullOrEmpty(referer))
			return new RedirectResult(referer);

		return new RedirectToActionResult("Index", "Home", null);
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
