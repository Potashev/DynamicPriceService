using DynamicPrice.Core.MediatR.CompanyEntity.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.MinimalAPI;

public static class CompaniesEndPoints
{
	public static void MapCompaniesEndPoints(this IEndpointRouteBuilder app)
	{
		var companies = app.MapGroup("/api/companies")
			.RequireAuthorization("CustomerPolicy");

		companies.MapGet("/", GetCompanies)
			.WithSummary("Получить список активных компаний");
		companies.MapGet("/{companyId}/products", GetCompanyProducts)
			.WithSummary("Получить продукты активной компании");
	}

	private static async Task<IResult> GetCompanies(
		[FromQuery] string? status,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		if (status == "active")
		{
			var activeCompanies = await mediator.Send(new GetActiveCompaniesQuery(), cancellationToken);
			return Results.Ok(activeCompanies);
		}

		return Results.StatusCode(StatusCodes.Status501NotImplemented); //todo: handle
	}

	private static async Task<IResult> GetCompanyProducts(
		string companyId,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		var companyProducts = await mediator.Send(new GetCompanyProductsQuery(companyId), cancellationToken);
		return Results.Ok(companyProducts);
	}
}
