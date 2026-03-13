using DynamicPrice.Core.MediatR.AuthEntity.Commands;
using DynamicPrice.Core.MediatR.ManagerEntity.Commands;
using DynamicPrice.Core.MediatR.ManagerEntity.Queries;
using DynamicPrice.Core.MediatR.ProductEntity.Queries;
using DynamicPrice.Shared.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.MinimalAPI;

public static class ManagerEndPoints
{
	public static void MapManagersEndPoints(this IEndpointRouteBuilder app)
	{
		var company = app.MapGroup("/api/company")
			.RequireAuthorization("ManagerPolicy");

		company.MapGet("/managers", GetCompanyManagers)
			.WithSummary("Получить информацию о компании менеджера");

		company.MapPost("/managers", RegisterManager)
			.WithSummary("Получить информацию о компании менеджера");


		company.MapGet("/info", GetCompanyInfo)
			.WithSummary("Получить информацию о компании менеджера");
	}

	private static async Task<IResult> GetCompanyManagers(
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var managersVm = await mediator.Send(new GetCompanyManagersQuery(), cancellationToken);
		return Results.Ok(managersVm);
	}

	private static async Task<IResult> RegisterManager(
		[FromBody] RegisterRequest registerVm,
		IMediator mediator)
	{
		await mediator.Send(new RegisterManagerCommand(registerVm));
		return Results.Ok("Manager registered successfully");
	}

	private static async Task<IResult> GetCompanyInfo(
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var managerInfo = await mediator.Send(new GetManagerInfoQuery(), cancellationToken);
		return Results.Ok(managerInfo.Company);
	}
}
