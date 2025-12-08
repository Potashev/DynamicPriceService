using System.Reflection;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using DynamicPrice.Core.SignalR;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace DynamicPrice.Tests.Services;

public class IncreasePriceServiceTests
{
	[Fact]
	public void IncreasePrice_PrivateMethod_ShouldIncreaseProductPriceCorrectly()
	{
		// Arrange
		var priceRule = new PriceRule { Increase = 10.0 }; // 10%
		var product = new Product { ProductId = 1, Price = 100m };
		var orderItem = new OrderItem { Product = product, Quantity = 2 };
		var service = CreateServiceForTests();

		// Act - invoke private method via reflection
		var method = typeof(IncreasePriceService).GetMethod("IncreasePrice", BindingFlags.NonPublic | BindingFlags.Instance);
		method.Should().NotBeNull();
		method!.Invoke(service, new object[] { new[] { orderItem }, priceRule });

		// Assert
		// increase = 100 * 10% * 2 = 20 -> final price = 120
		product.Price.Should().Be(120m);
	}

	[Fact]
	public async Task NoticeOfIncrease_PrivateMethod_ShouldCallHubSendAsyncForEachProduct()
	{
		// Arrange
		var product = new Product { ProductId = 5, Price = 50m };
		var orderItem = new OrderItem { Product = product, Quantity = 1 };

		var hubContextMock = new Mock<IHubContext<PriceHub>>();
		var clientsMock = new Mock<IHubClients>();
		var clientProxyMock = new Mock<IClientProxy>();

		clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(clientProxyMock.Object);
		hubContextMock.SetupGet(h => h.Clients).Returns(clientsMock.Object);
		clientProxyMock.Setup(cp => cp.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		var loggerMock = Mock.Of<Microsoft.Extensions.Logging.ILogger<IncreasePriceService>>();
		var service = new IncreasePriceService(serviceProvider: null!, hubContextMock.Object, loggerMock);

		var method = typeof(IncreasePriceService).GetMethod("NoticeOfIncrease", BindingFlags.NonPublic | BindingFlags.Instance);
		method.Should().NotBeNull();

		// Act
		var task = (Task)method!.Invoke(service, new object[] { new[] { orderItem } })!;
		await task;

		// Assert - verify SendCoreAsync called with proper args
		clientProxyMock.Verify(cp => cp.SendCoreAsync(
			"ReceivePriceUpdate",
			It.Is<object[]>(o => (int)o[0] == product.ProductId && (decimal)o[1] == product.Price),
			It.IsAny<CancellationToken>()), Times.Once);
	}

	private static IncreasePriceService CreateServiceForTests()
	{
		var hubContextMock = new Mock<IHubContext<PriceHub>>();
		var loggerMock = Mock.Of<Microsoft.Extensions.Logging.ILogger<IncreasePriceService>>();
		return new IncreasePriceService(serviceProvider: null!, hubContextMock.Object, loggerMock);
	}
}
