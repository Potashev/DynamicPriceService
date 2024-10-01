using DynamicPriceCore.Data;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.Extensions;

public static class MigrationExtensions
{
	public static void ApplyMigrations(this IApplicationBuilder app)
	{
		using var scope = app.ApplicationServices.CreateScope();
		using var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
		context.Database.Migrate();
	}
}
