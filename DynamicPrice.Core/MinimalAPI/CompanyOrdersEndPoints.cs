using DynamicPrice.Core.MediatR.OrderEntity.Commands;
using DynamicPrice.Core.MediatR.OrderEntity.Queries;
using MediatR;

namespace DynamicPrice.Core.MinimalAPI;

public static class CompanyOrdersEndPoints
{
	public static void MapCompanyOrdersEndPoints(this IEndpointRouteBuilder app)
	{
		var companyOrders = app.MapGroup("/api/company/orders")
			.RequireAuthorization("ManagerPolicy");

		companyOrders.MapGet("/", GetCompanyOrders)
			.WithSummary("Get all products");
		companyOrders.MapGet("/{orderId}", GetCompanyOrder)
			.WithSummary("Get all products");
		companyOrders.MapGet("/by-receive-key/{key}", GetOrderByReceiveKey)
			.WithSummary("Get all products");
		companyOrders.MapPatch("/{orderId}/ready", ReadyForReceive)
			.WithSummary("Edits a product");
		companyOrders.MapPatch("/{orderId}/complete", CompleteOrder)
			.WithSummary("Edits a product");
		companyOrders.MapGet("/statistics", GetCompanyStatistics)
			.WithSummary("Get all products");
	}

	private static async Task<IResult> GetCompanyOrders(IMediator mediator, CancellationToken cancellationToken)
	{
		var ordersVm = await mediator.Send(new GetCompanyOrdersQuery(), cancellationToken);
		return Results.Ok(ordersVm);
	}

	private static async Task<IResult> GetCompanyOrder(string orderId, IMediator mediator)
	{
		var orderVm = await mediator.Send(new GetCompanyOrderDetailsQuery(orderId));
		return Results.Ok(orderVm);
	}

	private static async Task<IResult> GetOrderByReceiveKey(string key, IMediator mediator)
	{
		var orderId = await mediator.Send(new GetOrderIdByReceiveKeyQuery(key));
		return Results.Ok(orderId);
	}

	private static async Task<IResult> ReadyForReceive(string orderId, IMediator mediator)
	{
		await mediator.Send(new ReadyForReceiveOrderCommand(orderId));
		return Results.Ok();
	}

	private static async Task<IResult> CompleteOrder(string orderId, IMediator mediator)
	{
		var id = await mediator.Send(new CompleteOrderCommand(orderId));
		return Results.Ok(id);
	}

	private static async Task<IResult> GetCompanyStatistics(IMediator mediator, CancellationToken cancellationToken)
	{
		var orderStatistics = await mediator.Send(new GetCompanyStatisticsQuery(), cancellationToken);
		return Results.Ok(orderStatistics);
	}
}
