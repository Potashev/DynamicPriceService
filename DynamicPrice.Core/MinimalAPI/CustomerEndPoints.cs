using DynamicPrice.Core.MediatR.CustomerEntity.Commands;
using DynamicPrice.Core.MediatR.CustomerEntity.Queries;
using DynamicPrice.Shared.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.MinimalAPI;

public static class CustomerEndPoints
{
	public static void MapCustomerEndPoints(this IEndpointRouteBuilder app)
	{
		var customers = app.MapGroup("/api/customers");

		customers.MapPost("", RegisterCustomer)
			.WithSummary("");

		var customer = customers.MapGroup("/me")
			.RequireAuthorization("CustomerPolicy");

		customer.MapGet("", GetCustomerInfo)
			.WithSummary("Получить информацию о текущем клиенте");

		customer.MapPut("/balance", TopUp)
			.WithSummary("Пополнить баланс клиента");
	}

	private static async Task<IResult> RegisterCustomer(
		[FromBody] RegisterRequest registerVm,
		IMediator mediator)
	{
		await mediator.Send(new RegisterCustomerCommand(registerVm));
		return Results.Ok("User registered successfully");
	}

	private static async Task<IResult> GetCustomerInfo(
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var customerInfo = await mediator.Send(new GetCustomerInfoQuery(), cancellationToken);
		return Results.Ok(customerInfo);
	}

	private static async Task<IResult> TopUp(
		BalanceRequest balanceVm,
		IMediator mediator)
	{
		await mediator.Send(new TopUpBalanceCommand(balanceVm));
		return Results.Ok();
	}
}
