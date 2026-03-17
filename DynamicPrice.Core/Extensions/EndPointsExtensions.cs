using DynamicPrice.Core.MinimalAPI;

namespace DynamicPrice.Core.Extensions;

public static class EndPointsExtensions
{
	public static void MapEndPoints(this IEndpointRouteBuilder app)
	{
		app.MapManagersEndPoints();
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