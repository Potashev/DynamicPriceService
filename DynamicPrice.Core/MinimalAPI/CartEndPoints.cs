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

		cart.MapGet("/", GetCartDetails)
			.WithSummary("Get all products");
		cart.MapPost("/items", AddProduct)
			.WithSummary("Creates a product");
		cart.MapDelete("/items/{productId}", RemoveProduct)
			.WithSummary("Creates a product");

		//products.MapGet("/{id}", GetProduct)
		//	.WithSummary("Get a product by id");
		//products.MapPut("/{id}", Edit)
		//	.WithSummary("Edits a product");
		//products.MapPost("/", Create)
		//	.WithSummary("Creates a product");
		//products.MapDelete("/{id}", Delete)
		//	.WithSummary("Deletes a product");
	}

	//todo: used TypedResults

	//[HttpGet]
	private static async Task<IResult> GetCartDetails([FromQuery(Name = "company-id")] string companyId, IMediator mediator, CancellationToken cancellationToken)
	{
		var cart = await mediator.Send(new GetCartDetailsQuery(Convert.ToInt32(companyId)), cancellationToken);
		return Results.Ok(cart);
	}

	//[HttpPost("items")]
	private static async Task<IResult> AddProduct([FromBody] int? productId, IMediator mediator)
	{
		var cart = await mediator.Send(new AddProductToCartCommand(productId.ToString()));
		return Results.Ok(cart.Company.CompanyId);
	}

	//[HttpDelete("items/{productId}")]
	private static async Task<IResult> RemoveProduct(int? productId, IMediator mediator)
	{
		var cart = await mediator.Send(new RemoveProductFromCartCommand(productId.ToString()));
		return Results.Ok(cart.Company.CompanyId);
	}
}
