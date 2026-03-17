using DynamicPrice.Core.MediatR.OrderEntity.Commands;
using DynamicPrice.Core.MediatR.OrderEntity.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.MinimalAPI;

public static class CustomerOrderEndPoints
{
	public static void MapCustomerOrderEndPoints(this IEndpointRouteBuilder app)
	{
		var customerOrder = app.MapGroup("/api/customer/order")
			.RequireAuthorization("CustomerPolicy");

		customerOrder.MapGet("", GetCustomerOrder)
			.WithSummary("Получить детали заказа клиента");

		customerOrder.MapPost("/confirm", ConfirmOrder)
			.WithSummary("Подтвердить заказ");

		customerOrder.MapPatch("/cancel", CancelOrder)
			.WithSummary("Отменить заказ");
	}

	private static async Task<IResult> GetCustomerOrder(
		[FromQuery(Name = "id")] string orderId,
		IMediator mediator,
		CancellationToken cancellationToken)
			=> Results.Ok(await mediator.Send(new GetCustomerOrderDetailsQuery(orderId), cancellationToken));

	private static async Task<IResult> ConfirmOrder(
		[FromBody] int cartId,
		IMediator mediator,
		CancellationToken cancellationToken)
			=> Results.Ok(await mediator.Send(new ConfirmOrderCommand(cartId), cancellationToken));

	private static async Task<IResult> CancelOrder(
		[FromBody] int orderId,
		IMediator mediator,
		CancellationToken cancellationToken)
			=> Results.Ok(await mediator.Send(new CancelOrderCommand(orderId), cancellationToken));
}
