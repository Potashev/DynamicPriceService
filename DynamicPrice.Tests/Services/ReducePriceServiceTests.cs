using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DynamicPrice.Tests.Services;

public class ReducePriceServiceTests
{
	[Fact]
	public void ReducePrice_PrivateMethod_ShouldReduceDecimalPriceCorrectly()
	{
		// Arrange
		var service = CreateServiceForTests();

		var method = typeof(ReducePriceService).GetMethod("ReducePrice", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new[] { typeof(decimal), typeof(double), typeof(bool) }, null);
		method.Should().NotBeNull();

		// Act
		var result = (decimal)method!.Invoke(service, new object[] { 100m, 10.0, false })!;

		// Assert: 10% reduction -> 90
		result.Should().Be(90m);
	}

	[Fact]
	public async Task ReducePrice_PrivateAsyncMethod_ShouldUpdateProduct_AddPriceDynamic_AndNotifyHub()
	{
		// Arrange
		var dbName = "ReducePriceTestDb" + System.Guid.NewGuid();

		var services = new ServiceCollection();
		services.AddDbContext<DynamicPriceCoreContext>(opts => opts.UseInMemoryDatabase(dbName));

		var hubContextMock = new Mock<IHubContext<PriceHub>>();
		var clientsMock = new Mock<IHubClients>();
		var clientProxyMock = new Mock<IClientProxy>();
		clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(clientProxyMock.Object);
		hubContextMock.SetupGet(h => h.Clients).Returns(clientsMock.Object);
		clientProxyMock.Setup(cp => cp.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		services.AddSingleton(hubContextMock.Object);

		var serviceProvider = services.BuildServiceProvider();

		// Seed data
		using (var scope = serviceProvider.CreateScope())
		{
			var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

			var company = new Company { CompanyId = 111, Title = "Comp 111" };
			await context.Companies.AddAsync(company);

			var product = new Product
			{
				ProductId = 777,
				CompanyId = company.CompanyId,
				Company = company,
				Title = "Product777",
				Price = 200m,
				MinimumPrice = 50m,
				Quantity = 10
			};

			await context.Products.AddAsync(product);

			var priceRule = new PriceRule
			{
				PriceRuleId = 1,
				Company = company,
				Increase = 0,
				Reduction = 10.0
			};

			await context.PriceRules.AddAsync(priceRule);

			await context.SaveChangesAsync();
		}

		var loggerMock = Mock.Of<Microsoft.Extensions.Logging.ILogger<ReducePriceService>>();
		var configMock = Mock.Of<Microsoft.Extensions.Configuration.IConfiguration>();

		var service = new ReducePriceService(serviceProvider, configMock, hubContextMock.Object, loggerMock);

		var method = typeof(ReducePriceService).GetMethod("ReducePrice", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new[] { typeof(int) }, null);
		method.Should().NotBeNull();

		// Act
		var task = (Task)method!.Invoke(service, new object[] { 777 })!;
		await task;

		// Assert: check db changes
		using (var scope = serviceProvider.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var productInDb = await ctx.Products.FirstOrDefaultAsync(p => p.ProductId == 777);
			productInDb.Should().NotBeNull();
			productInDb!.Price.Should().BeGreaterThanOrEqualTo(productInDb.MinimumPrice);

			var dyn = await ctx.PriceDynamics.FirstOrDefaultAsync(d => d.ProductId == 777);
			dyn.Should().NotBeNull();
		}

		// verify hub notify called
		clientProxyMock.Verify(cp => cp.SendCoreAsync(
			"ReceivePriceUpdate",
			It.Is<object[]>(o => (int)o[0] == 777 && (decimal)o[1] >= 0m),
			It.IsAny<CancellationToken>()), Times.AtLeastOnce);
	}

	private static ReducePriceService CreateServiceForTests()
	{
		var hubContextMock = new Mock<IHubContext<PriceHub>>();
		var loggerMock = Mock.Of<Microsoft.Extensions.Logging.ILogger<ReducePriceService>>();
		var configMock = Mock.Of<Microsoft.Extensions.Configuration.IConfiguration>();
		return new ReducePriceService(serviceProvider: null!, configMock, hubContextMock.Object, loggerMock);
	}
}
