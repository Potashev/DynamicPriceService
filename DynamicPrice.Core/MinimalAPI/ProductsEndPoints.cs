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

	private static async Task<IResult> GetProducts(
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var hash = cancellationToken.GetHashCode();

		var productsVm = await mediator.Send(new GetProductsQuery(), cancellationToken);
		return Results.Ok(productsVm);
	}

	private static async Task<IResult> GetProduct(
		int id,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var productVm = await mediator.Send(new GetProductDetailsQuery(id), cancellationToken);
		return Results.Ok(productVm);
	}

	private static async Task<IResult> Edit(
		ProductViewModel productVm,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var productId = await mediator.Send(new EditProductCommand(productVm), cancellationToken);
		return Results.Ok(productId);
	}

	private static async Task<IResult> Create(
		ProductViewModel productVm,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var productId = await mediator.Send(new CreateProductCommand(productVm), cancellationToken);
		return Results.Ok(productId);
	}

	private static async Task<IResult> Delete(
		int id,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		await mediator.Send(new DeleteProductCommand(id), cancellationToken);
		return Results.Ok();
	}
}
