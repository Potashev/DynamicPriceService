namespace DynamicPrice.Core.MinimalAPI;

public static class EndPoints
{
	public static void MapEndPoints(this IEndpointRouteBuilder app)
	{
		app.MapProductsEndPoints();
		app.MapPriceRuleEndPoints();
		app.MapManagerEndPoints();
		app.MapCompanyOrdersEndPoints();
		app.MapCartEndPoints();

		app.MapAuthEndPoints();
	}
}