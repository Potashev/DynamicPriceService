using DynamicPrice.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Extensions;

public static class MigrationExtensions
{
	public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
	{
		using var scope = app.ApplicationServices.CreateScope();
		var serviceProvider = scope.ServiceProvider;

		var dynamicPriceContext = serviceProvider.GetRequiredService<DynamicPriceCoreContext>();
		await dynamicPriceContext.Database.MigrateAsync();	//<- здесь

		var identityContext = serviceProvider.GetRequiredService<IdentityContext>();
		await identityContext.Database.MigrateAsync();

		await DbInitializer.SeedDataAsync(serviceProvider);
	}
}
