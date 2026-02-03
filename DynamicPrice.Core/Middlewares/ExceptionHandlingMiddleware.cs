using DynamicPrice.Core.Exceptions;
using System.Diagnostics;
using System.Text.Json;

namespace DynamicPrice.Core.Middlewares;

public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;
	private readonly IHostEnvironment _env;

	public ExceptionHandlingMiddleware(
		RequestDelegate next,
		ILogger<ExceptionHandlingMiddleware> logger,
		IHostEnvironment env)
	{
		_next = next;
		_logger = logger;
		_env = env;
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
				context.Response.ContentType = "application/problem+json";

				var (status, title) = MapExceptionToProblem(ex);

				context.Response.StatusCode = status;

				var problem = new
				{
					type = $"https://httpstatuses.com/{status}",
					title,
					status,
					detail = _env.IsDevelopment() ? ex.Message : null,
					instance = context.Request.Path,
					traceId = Activity.Current?.Id ?? context.TraceIdentifier
				};

				var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
				await context.Response.WriteAsync(json);
			}
		}
	}

	private static (int status, string title) MapExceptionToProblem(Exception ex)
	{
		return ex switch
		{
			NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
			ValidationException => (StatusCodes.Status400BadRequest, "Bad Request"),
			UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
			BusinessException => (StatusCodes.Status409Conflict, "Conflict"),
			ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
			_ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
		};
	}
}