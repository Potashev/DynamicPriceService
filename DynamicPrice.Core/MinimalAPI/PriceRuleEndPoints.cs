using DynamicPrice.Core.MediatR.PriceRuleEntity.Commands;
using DynamicPrice.Core.MediatR.PriceRuleEntity.Queries;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MinimalAPI;

public static class PriceRuleEndPoints
{
	public static void MapPriceRuleEndPoints(this IEndpointRouteBuilder app)
	{
		var priceRule = app.MapGroup("/api/company/price-rule")
			.RequireAuthorization("ManagerPolicy");

		priceRule.MapGet("/", Get)
			.WithSummary("Получить правило ценообразования и его статус");
		priceRule.MapPut("/", Edit)
			.WithSummary("Изменить правило ценообразования");
		priceRule.MapPost("/run", RunPriceReducing)
			.WithSummary("Запустить снижение цены на продукты компании");
		priceRule.MapPost("/stop", StopPriceReducing)
			.WithSummary("Остановить снижение цены на продукты компании");
	}

	//todo: pass cancellation

	private static async Task<IResult> Get(IMediator mediator)
	{
		var result = await mediator.Send(new GetPriceRuleWithStatusQuery());
		return Results.Ok(result);
	}

	private static async Task<IResult> Edit(
		PriceRuleViewModel priceRuleVm,
		IMediator mediator)
	{
		var result = await mediator.Send(new EditPriceRuleCommand(priceRuleVm));
		return Results.Ok(result);
	}

	private static async Task<IResult> RunPriceReducing(IMediator mediator)
	{
		await mediator.Send(new PriceReducingCommand(true));
		return Results.Ok();
	}

	private static async Task<IResult> StopPriceReducing(IMediator mediator)
	{
		await mediator.Send(new PriceReducingCommand(false));
		return Results.Ok();
	}
}
