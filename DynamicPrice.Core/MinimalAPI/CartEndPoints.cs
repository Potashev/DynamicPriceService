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

		cart.MapPost("/items", AddProduct)
			.WithSummary("Добавить товар в корзину");

		cart.MapDelete("/items/{productId}", RemoveProduct)
			.WithSummary("Удалить товар из корзины");
	}

	private static async Task<IResult> GetCartDetails(
		[FromQuery(Name = "company-id")] string companyId,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var cart = await mediator.Send(new GetCartDetailsQuery(Convert.ToInt32(companyId)), cancellationToken);
		return Results.Ok(cart);
	}

	private static async Task<IResult> AddProduct(
		[FromBody] int productId,
		IMediator mediator)
	{
		var cart = await mediator.Send(new AddProductToCartCommand(productId.ToString()));
		return Results.Ok(cart.Company.CompanyId);
	}

	private static async Task<IResult> RemoveProduct(
		int productId,
		IMediator mediator)
	{
		var cart = await mediator.Send(new RemoveProductFromCartCommand(productId.ToString()));
		return Results.Ok(cart.Company.CompanyId);
	}
}
