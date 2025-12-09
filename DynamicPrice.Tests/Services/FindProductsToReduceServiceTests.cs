using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
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
	public async Task FindProductsToReduce_ShouldReturnProducts_WhenLastSellTimeExceeded()
	{
		var dbName = "FindProductsToReduceTestDb1" + Guid.NewGuid();
		var sp = BuildServices(dbName);

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

			var company = new Company { CompanyId = 1, Title = "C1" };
			await ctx.Companies.AddAsync(company);
			await ctx.ActiveCompanies.AddAsync(new ActiveCompany { CompanyId = company.CompanyId, StartedAt = DateTime.UtcNow, LastMonitoring = DateTime.UtcNow });

			var priceRule = new PriceRule { PriceRuleId = 1, Company = company, CompanyId = company.CompanyId, NoSellSeconds = 10 };
			await ctx.PriceRules.AddAsync(priceRule);

			var oldProduct = new Product { ProductId = 100, Company = company, CompanyId = company.CompanyId, Title = "Old", Price = 10m, LastSellTime = DateTime.UtcNow.AddSeconds(-30) };
			var recentProduct = new Product { ProductId = 101, Company = company, CompanyId = company.CompanyId, Title = "Recent", Price = 5m, LastSellTime = DateTime.UtcNow };

			await ctx.Products.AddRangeAsync(oldProduct, recentProduct);
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

		results.Select(r => r.ProductId).Should().Contain(100).And.NotContain(101);
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

	[Fact]
	public async Task FindProductsToReduce_ShouldRespectProductsCountParameter()
	{
		var dbName = "FindProductsToReduceTestDb3" + Guid.NewGuid();
		var sp = BuildServices(dbName);

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

			var company = new Company { CompanyId = 3, Title = "C3" };
			await ctx.Companies.AddAsync(company);
			await ctx.ActiveCompanies.AddAsync(new ActiveCompany { CompanyId = company.CompanyId, StartedAt = DateTime.UtcNow, LastMonitoring = DateTime.UtcNow });

			var priceRule = new PriceRule { PriceRuleId = 3, Company = company, CompanyId = company.CompanyId, NoSellSeconds = 1 };
			await ctx.PriceRules.AddAsync(priceRule);

			for (int i = 0; i < 5; i++)
			{
				var p = new Product { ProductId = 300 + i, Company = company, CompanyId = company.CompanyId, Title = "P" + i, Price = 1m, LastSellTime = DateTime.UtcNow.AddSeconds(-30) };
				await ctx.Products.AddAsync(p);
			}

			await ctx.SaveChangesAsync();
		}

		var service = new FindProductsToReduceService(serviceProvider: null!, config: null!, logger: Mock.Of<Microsoft.Extensions.Logging.ILogger<FindProductsToReduceService>>());

		var method = typeof(FindProductsToReduceService).GetMethod("FindProductsToReduceAsync", BindingFlags.NonPublic | BindingFlags.Instance);
		method.Should().NotBeNull();

		List<Product> results = [];
		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var enumerable = (IAsyncEnumerable<Product>)method!.Invoke(service, new object[] { ctx, CancellationToken.None, 2 })!;
			await foreach (var p in enumerable.WithCancellation(CancellationToken.None))
				results.Add(p);
		}

		results.Count.Should().Be(2);
	}
}
