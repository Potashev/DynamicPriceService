using DynamicPrice.Core.MediatR.ProductEntity.Commands;
using DynamicPrice.Core.MediatR.ProductEntity.Queries;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MinimalAPI;

public static class ProductsEndPoints
{
	public static void MapProductsEndPoints(this IEndpointRouteBuilder app)
	{
		var products = app.MapGroup("/api/company/products")
			.RequireAuthorization("ManagerPolicy");

		products.MapGet("", GetProducts)
			.WithSummary("Получить все продукт");

		products.MapGet("/{id}", GetProduct)
			.WithSummary("Получить продукт по идентификатору");

		products.MapPut("/{id}", Edit)
			.WithSummary("Редактировать продукт");

		products.MapPost("", Create)
			.WithSummary("Создать продукт");

		products.MapDelete("/{id}", Delete)
			.WithSummary("Удалить продукт");
	}

	//todo: used TypedResults

	private static async Task<IResult> GetProducts(
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var productsVm = await mediator.Send(new GetProductsQuery(), cancellationToken);
		return Results.Ok(productsVm);
	}

	private static async Task<IResult> GetProduct(
		int id,
		IMediator mediator)
	{
		var productVm = await mediator.Send(new GetProductDetailsQuery(id));
		return Results.Ok(productVm);
	}

	private static async Task<IResult> Edit(
		int id,
		ProductViewModel productVm,
		IMediator mediator)
	{
		if (id != productVm.ProductId)
		{
			return Results.BadRequest();
		}
		var productId = await mediator.Send(new EditProductCommand(productVm));
		return Results.Ok(productId);
	}

	private static async Task<IResult> Create(
		ProductViewModel productVm,
		IMediator mediator)
	{
		var productId = await mediator.Send(new CreateProductCommand(productVm));
		return Results.Ok(productId);
	}

	private static async Task<IResult> Delete(
		int id,
		IMediator mediator)
	{
		await mediator.Send(new DeleteProductCommand(id));
		return Results.Ok();
	}
}
