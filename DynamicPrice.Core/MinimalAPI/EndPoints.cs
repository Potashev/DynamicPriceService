namespace DynamicPrice.Core.MinimalAPI;

public static class EndPoints
{
	public static void MapEndPoints(this IEndpointRouteBuilder app)
	{
		app.MapManagerEndPoints();
		app.MapProductsEndPoints();
		app.MapPriceRuleEndPoints();
		app.MapCompanyOrdersEndPoints();

		app.MapCustomerEndPoints();
		app.MapCompaniesEndPoints();
		app.MapCartEndPoints();
		app.MapCustomerOrderEndPoints();

		app.MapAuthEndPoints();
	}
}