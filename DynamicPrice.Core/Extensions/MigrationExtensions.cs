using DynamicPrice.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Extensions;

public static class MigrationExtensions
{
	public static async void ApplyMigrations(this IApplicationBuilder app)
	{
		using var scope = app.ApplicationServices.CreateScope();
		var services = scope.ServiceProvider;

		var dynamicPriceDb = services.GetRequiredService<DynamicPriceCoreContext>();
		dynamicPriceDb.Database.Migrate();

		var identityDb = services.GetRequiredService<IdentityContext>();
		identityDb.Database.Migrate();

		await DbInitializer.SeedDataAsync(services);
	}
}
