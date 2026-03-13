using DynamicPrice.Core.MediatR.ManagerEntity.Queries;
using MediatR;

namespace DynamicPrice.Core.MinimalAPI;

public static class ManagerEndPoints
{
	public static void MapManagerEndPoints(this IEndpointRouteBuilder app)
	{
		app.MapGet("/api/company/info", GetCompanyInfo)
			.RequireAuthorization("ManagerPolicy")
			.WithSummary("Получить информацию о компании менеджера");

		// get company/managers
		// post company/managers
	}

	private static async Task<IResult> GetCompanyInfo(
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var managerInfo = await mediator.Send(new GetManagerInfoQuery(), cancellationToken);
		return Results.Ok(managerInfo.Company);
	}
}
