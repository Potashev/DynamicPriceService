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

		products.MapPatch("/{id}/archive", MakeArchived)
			.WithSummary("");

		products.MapPatch("/{id}/activate", MakeActive)
			.WithSummary("");
	}

	private static async Task<IResult> GetProducts(
		IMediator mediator,
		CancellationToken cancellationToken)
			=> Results.Ok(await mediator.Send(new GetProductsQuery(), cancellationToken));

	private static async Task<IResult> GetProduct(
		int id,
		IMediator mediator,
		CancellationToken cancellationToken)
			=> Results.Ok(await mediator.Send(new GetProductDetailsQuery(id), cancellationToken));

	private static async Task<IResult> Edit(
		ProductViewModel productVm,
		IMediator mediator,
		CancellationToken cancellationToken)
			=> Results.Ok(await mediator.Send(new EditProductCommand(productVm), cancellationToken));

	private static async Task<IResult> Create(
		ProductViewModel productVm,
		IMediator mediator,
		CancellationToken cancellationToken)
			=> Results.Ok(await mediator.Send(new CreateProductCommand(productVm), cancellationToken));

	private static async Task<IResult> MakeArchived(
		int id,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		await mediator.Send(new MakeArchivedProductCommand(id), cancellationToken);
		return Results.Ok();
	}

	private static async Task<IResult> MakeActive(
		int id,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		await mediator.Send(new MakeActiveProductCommand(id), cancellationToken);
		return Results.Ok();
	}
}
