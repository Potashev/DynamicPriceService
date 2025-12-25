namespace DynamicPrice.Core.MinimalAPI;

public static class EndPoints
{
	public static void MapEndPoints(this IEndpointRouteBuilder app)
	{
		app.MapProductEndPoints();
		app.MapPriceRuleEndPoints();
		app.MapManagerEndPoints();
	}
}