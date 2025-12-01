using DynamicPrice.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Extensions;

public static class MigrationExtensions
{
	public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
	{
		using var scope = app.ApplicationServices.CreateScope();
		var services = scope.ServiceProvider;

		var dynamicPriceDb = services.GetRequiredService<DynamicPriceCoreContext>();
		await dynamicPriceDb.Database.MigrateAsync();

		var identityDb = services.GetRequiredService<IdentityContext>();
		await identityDb.Database.MigrateAsync();

		await DbInitializer.SeedDataAsync(services);
	}
}
