using DynamicPrice.Core.MediatR.CustomerEntity.Commands;
using DynamicPrice.Core.MediatR.CustomerEntity.Queries;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.MinimalAPI;

public static class CustomerEndPoints
{
	public static void MapCustomerEndPoints(this IEndpointRouteBuilder app)
	{
		var customer = app.MapGroup("/api/customer")
			.RequireAuthorization("CustomerPolicy");

		customer.MapGet("/me", GetCustomerInfo)
			.WithSummary("Get all products");
		customer.MapPut("/me/balance", TopUp)
			.WithSummary("Edits a product");
	}

	private static async Task<IResult> GetCustomerInfo(IMediator mediator, CancellationToken cancellationToken)
	{
		var customerInfo = await mediator.Send(new GetCustomerInfoQuery(), cancellationToken);
		return Results.Ok(customerInfo);
	}

	private static async Task<IResult> TopUp([FromBody] BalanceViewModel balanceVm, IMediator mediator)
	{
		await mediator.Send(new TopUpBalanceCommand(balanceVm));
		return Results.Ok();
	}
}
