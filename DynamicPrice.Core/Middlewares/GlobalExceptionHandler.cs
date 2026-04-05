using DynamicPrice.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.Middlewares;

internal sealed class GlobalExceptionHandler(
	IProblemDetailsService problemDetailsService,
	ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		logger.LogError(exception, "Unhandled exception occurred");

		httpContext.Response.StatusCode = exception switch
		{
			ValidationException => StatusCodes.Status400BadRequest,
			UnauthorizedException => StatusCodes.Status401Unauthorized,
			NotFoundException => StatusCodes.Status404NotFound,
			BusinessException => StatusCodes.Status409Conflict,
			_ => StatusCodes.Status500InternalServerError
		};

		return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
		{
			HttpContext = httpContext,
			Exception = exception,
			ProblemDetails = new ProblemDetails
			{
				Type = exception.GetType().Name,
				Title = "An error occured",
				Detail = exception.Message
			}
		});
	}
}
