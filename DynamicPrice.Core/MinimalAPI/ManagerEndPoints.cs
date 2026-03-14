using DynamicPrice.Core.MediatR.ManagerEntity.Commands;
using DynamicPrice.Core.MediatR.ManagerEntity.Queries;
using DynamicPrice.Shared.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.MinimalAPI;

public static class ManagerEndPoints
{
	public static void MapManagersEndPoints(this IEndpointRouteBuilder app)
	{
		var company = app.MapGroup("/api/company/managers")
			.RequireAuthorization("ManagerPolicy");

		company.MapGet("", GetCompanyManagers)
			.WithSummary("Получить список менеджеров компании");

		company.MapPost("", RegisterManager)
			.WithSummary("Регистрация нового менеджера");

		company.MapGet("/me", GetManagerInfo)
			.WithSummary("Получить информацию о текущем менеджере");
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

	private static async Task<IResult> GetManagerInfo(
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var managerVm = await mediator.Send(new GetManagerInfoQuery(), cancellationToken);
		return Results.Ok(managerVm);
	}
}
