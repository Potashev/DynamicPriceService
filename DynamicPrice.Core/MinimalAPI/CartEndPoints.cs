using DynamicPrice.Core.MediatR.CartEntity.Commands;
using DynamicPrice.Core.MediatR.CartEntity.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.MinimalAPI;

public static class CartEndPoints
{
	public static void MapCartEndPoints(this IEndpointRouteBuilder app)
	{
		var cart = app.MapGroup("/api/cart")
			.RequireAuthorization("CustomerPolicy");

		cart.MapGet("", GetCartDetails)
			.WithSummary("Получить детали корзины клиента");

		cart.MapPost("/items", AddCartItem)
			.WithSummary("Добавить товар в корзину");

		cart.MapDelete("/items/{productId}", RemoveCartItem)
			.WithSummary("Удалить товар из корзины");
	}

	private static async Task<IResult> GetCartDetails(
		[FromQuery(Name = "company-id")] Guid companyId,	//todo: check
		IMediator mediator,
		CancellationToken cancellationToken)
			=> Results.Ok(await mediator.Send(new GetCartDetailsQuery(companyId), cancellationToken));

	private static async Task<IResult> AddCartItem(
		[FromBody] int productId,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var cart = await mediator.Send(new AddProductToCartCommand(productId.ToString()), cancellationToken);
		return Results.Ok(cart.CompanyId);
	}

	private static async Task<IResult> RemoveCartItem(
		Guid productId,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var cart = await mediator.Send(new RemoveProductFromCartCommand(productId), cancellationToken);
		return Results.Ok(cart.CompanyId);
	}
}
