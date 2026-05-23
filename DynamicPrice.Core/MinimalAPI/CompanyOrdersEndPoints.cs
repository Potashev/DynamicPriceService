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

		companyOrders.MapGet("", GetCompanyOrders)
			.WithSummary("Получить заказы компании");

		companyOrders.MapGet("/{orderId}", GetCompanyOrder)
			.WithSummary("Получить детали заказа компании");

		companyOrders.MapGet("/by-receive-key/{key}", GetOrderByReceiveKey)
			.WithSummary("Получить заказ по ключу получения");

		companyOrders.MapPatch("/{orderId}/ready", ReadyForReceive)
			.WithSummary("Сделать заказ готовым к выдаче");

		companyOrders.MapPatch("/{orderId}/complete", CompleteOrder)
			.WithSummary("Завершить заказ");

		companyOrders.MapGet("/statistics", GetCompanyStatistics)
			.WithSummary("Получить статистику по заказам компании");
	}

	private static async Task<IResult> GetCompanyOrders(
		IMediator mediator,
		CancellationToken cancellationToken)
			=> Results.Ok(await mediator.Send(new GetCompanyOrdersQuery(), cancellationToken));

	private static async Task<IResult> GetCompanyOrder(
		Guid orderId,
		IMediator mediator,
		CancellationToken cancellationToken)
			=> Results.Ok(await mediator.Send(new GetCompanyOrderDetailsQuery(orderId), cancellationToken));

	private static async Task<IResult> GetOrderByReceiveKey(
		string key,
		IMediator mediator,
		CancellationToken cancellationToken)
			=> Results.Ok(await mediator.Send(new GetOrderIdByReceiveKeyQuery(key), cancellationToken));

	private static async Task<IResult> ReadyForReceive(
		Guid orderId,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		await mediator.Send(new ReadyForReceiveOrderCommand(orderId), cancellationToken);
		return Results.Ok();
	}

	private static async Task<IResult> CompleteOrder(
		string orderId,
		IMediator mediator,
		CancellationToken cancellationToken)
			=> Results.Ok(await mediator.Send(new CompleteOrderCommand(orderId), cancellationToken));

	private static async Task<IResult> GetCompanyStatistics(
		IMediator mediator,
		CancellationToken cancellationToken)
			=> Results.Ok(await mediator.Send(new GetCompanyStatisticsQuery(), cancellationToken));
}
