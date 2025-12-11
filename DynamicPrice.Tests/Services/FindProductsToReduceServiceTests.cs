using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection;

namespace DynamicPrice.Tests.Services;

public class FindProductsToReduceServiceTests
{
	private static ServiceProvider BuildServices(string dbName)
	{
		var services = new ServiceCollection();
		services.AddDbContext<DynamicPriceCoreContext>(opts => opts.UseInMemoryDatabase(dbName));
		return services.BuildServiceProvider();
	}

	[Fact]
	public async Task FindProductsToReduce_ShouldNotReturnProducts_ForInactiveCompanies()
	{
		var dbName = "FindProductsToReduceTestDb2" + Guid.NewGuid();
		var sp = BuildServices(dbName);

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

			var company = new Company { CompanyId = 2, Title = "C2" };
			await ctx.Companies.AddAsync(company);
			// Note: not adding ActiveCompany entry -> company is inactive

			var priceRule = new PriceRule { PriceRuleId = 2, Company = company, CompanyId = company.CompanyId, NoSellSeconds = 10 };
			await ctx.PriceRules.AddAsync(priceRule);

			var oldProduct = new Product { ProductId = 200, Company = company, CompanyId = company.CompanyId, Title = "Old2", Price = 10m, LastSellTime = DateTime.UtcNow.AddSeconds(-30) };
			await ctx.Products.AddAsync(oldProduct);
			await ctx.SaveChangesAsync();
		}

		var service = new FindProductsToReduceService(serviceProvider: null!, config: null!, logger: Mock.Of<Microsoft.Extensions.Logging.ILogger<FindProductsToReduceService>>());

		var method = typeof(FindProductsToReduceService).GetMethod("FindProductsToReduceAsync", BindingFlags.NonPublic | BindingFlags.Instance);
		method.Should().NotBeNull();

		List<Product> results = [];
		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var enumerable = (IAsyncEnumerable<Product>)method!.Invoke(service, new object[] { ctx, CancellationToken.None, null })!;
			await foreach (var p in enumerable.WithCancellation(CancellationToken.None))
				results.Add(p);
		}

		results.Should().BeEmpty();
	}
}
