using System.Text.Json;

namespace DynamicPrice.Core.Middlewares;

public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;

	public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unhandled exception processing request {Method} {Path}", context.Request.Method, context.Request.Path);
			if (!context.Response.HasStarted)
			{
				context.Response.Clear();
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				context.Response.ContentType = "application/problem+json";

				var problem = new
				{
					type = "https://httpstatuses.com/500",
					title = "Internal Server Error",
					status = 500,
					detail = ex.Message // для prod можно убрать подробности
				};

				var json = JsonSerializer.Serialize(problem);
				await context.Response.WriteAsync(json);
			}
		}
	}
}