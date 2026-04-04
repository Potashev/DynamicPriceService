using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DynamicPrice.Manager.Extension;

public static class ApiExceptionExtensions
{
	public static async Task<ProblemDetails?> GetProblemDetailsAsync(this Refit.ApiException ex)	// todo: check. Refit.ProblemDetails
	{
		if (string.IsNullOrWhiteSpace(ex.Content))
			return null;

		return JsonSerializer.Deserialize<ProblemDetails>(
			ex.Content,
			new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});
	}
}
