using DynamicPrice.Core.Data;
using DynamicPrice.Core.MediatR.OrderEntity.Commands;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using DynamicPrice.Tests.Fixtures;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DynamicPrice.Tests.MediatR.OrderEntity;

public class OrderHandlersTests
{
	[Fact]
	public async Task ConfirmOrder_ShouldCreateOrder_RemoveCart_AndDecreaseProductQuantity()
	{
		var dbName = TestDbHelper.NewDbName("ConfirmOrderTestDb");
		var sp = TestDbHelper.CreateServiceProvider(dbName);

		int cartId;
		// seed
		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var company = new Company { CompanyId = 10, Title = "C" };
			await ctx.Companies.AddAsync(company);
			var product = new Product { ProductId = 100, Company = company, CompanyId = company.CompanyId, Title = "Prod", Price = 10m, Quantity = 5 };
			await ctx.Products.AddAsync(product);

			var cart = new Cart { CustomerId = "cust1", Company = company, CartItems = [] };
			cart.CartItems.Add(new CartItem { Product = product, ProductId = product.ProductId, Quantity = 2, Cart = cart });
			await ctx.Carts.AddAsync(cart);
			await ctx.SaveChangesAsync();
			cartId = cart.CartId;
		}

		var userServiceMock = new Mock<IUserService>();
		userServiceMock.Setup(u => u.GetCurrentUserAsync()).ReturnsAsync(new ApplicationUser { Id = "cust1" });

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var handler = new ConfirmOrderCommandHandler(ctx, userServiceMock.Object);

			var result = await handler.Handle(new ConfirmOrderCommand(cartId), default);

			result.Should().BeGreaterThan(0);

			var orderInDb = await ctx.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == result);
			orderInDb.Should().NotBeNull();
			orderInDb!.OrderItems.Should().HaveCount(1);
			orderInDb.Status.Should().Be(OrderStatus.Confirmed);

			var cartExists = await ctx.Carts.AnyAsync(c => c.CartId == cartId);
			cartExists.Should().BeFalse();

			var productInDb = await ctx.Products.FirstOrDefaultAsync(p => p.ProductId == 100);
			productInDb!.Quantity.Should().Be(3); // 5 - 2
		}
	}

	[Fact]
	public async Task CompleteOrder_ShouldChargeCustomer_UpdateLastSellTime_SetStatus_AndPublishEvent()
	{
		var dbName = TestDbHelper.NewDbName("CompleteOrderTestDb");
		var sp = TestDbHelper.CreateServiceProvider(dbName);

		int orderId;

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var company = new Company { CompanyId = 20, Title = "Comp" };
			await ctx.Companies.AddAsync(company);

			var product = new Product { ProductId = 200, Company = company, CompanyId = company.CompanyId, Title = "P", Price = 10m, Quantity = 10 };
			await ctx.Products.AddAsync(product);

			var order = new Order
			{
				Number = "ORD-100",
				CustomerId = "cust2",
				Company = company,
				Status = OrderStatus.Ready,
				OrderDate = DateTime.UtcNow.AddMinutes(-5),
				OrderItems = []
			};
			order.OrderItems.Add(new OrderItem { Product = product, ProductId = product.ProductId, Quantity = 3, ProductPrice = product.Price });
			await ctx.Orders.AddAsync(order);
			await ctx.SaveChangesAsync();
			orderId = order.OrderId;
		}

		var publishMock = new Mock<IPublishEndpoint>();
		publishMock.Setup(p => p.Publish(It.IsAny<object>(), default)).Returns(Task.CompletedTask);

		var userServiceMock = new Mock<IUserService>();
		// manager performing completion
		userServiceMock.Setup(u => u.GetCurrentUserAsync()).ReturnsAsync(new ApplicationUser { Id = "man1", CompanyId = 20 });
		// customer that will be charged
		userServiceMock.Setup(u => u.GetUserByIdAsync("cust2")).ReturnsAsync(new ApplicationUser { Id = "cust2", Balance = 100m });
		userServiceMock.Setup(u => u.UpdateUserAsync(It.IsAny<ApplicationUser>())).Returns(Task.CompletedTask).Verifiable();

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var handler = new CompleteOrderCommandHandler(ctx, userServiceMock.Object, publishMock.Object);

			var result = await handler.Handle(new CompleteOrderCommand(orderId.ToString()), default);

			result.Should().Be(orderId);

			var orderInDb = await ctx.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);
			orderInDb!.Status.Should().Be(OrderStatus.Completed);
			orderInDb.ReceiveKey.Should().Be(0);

			var productInDb = await ctx.Products.FirstOrDefaultAsync(p => p.ProductId == 200);
			productInDb!.LastSellTime.Should().Be(orderInDb.OrderDate);

			userServiceMock.Verify(u => u.UpdateUserAsync(It.Is<ApplicationUser>(a => a.Id == "cust2" && a.Balance == 70m)), Times.Once);

			publishMock.Verify(p => p.Publish(It.IsAny<PriceIncreaseEvent>(), default), Times.Once);
		}
	}

	[Fact]
	public async Task CompleteOrder_ShouldThrow_WhenCustomerHasInsufficientBalance()
	{
		var dbName = TestDbHelper.NewDbName("CompleteOrderFailTestDb");
		var sp = TestDbHelper.CreateServiceProvider(dbName);

		int orderId;

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var company = new Company { CompanyId = 21, Title = "Comp2" };
			await ctx.Companies.AddAsync(company);

			var product = new Product { ProductId = 201, Company = company, CompanyId = company.CompanyId, Title = "P2", Price = 50m };
			await ctx.Products.AddAsync(product);

			var order = new Order
			{
				Number = "ORD-201",
				CustomerId = "cust3",
				Company = company,
				Status = OrderStatus.Ready,
				OrderDate = DateTime.UtcNow,
				OrderItems = []
			};
			order.OrderItems.Add(new OrderItem { Product = product, ProductId = product.ProductId, Quantity = 3, ProductPrice = product.Price });
			await ctx.Orders.AddAsync(order);
			await ctx.SaveChangesAsync();
			orderId = order.OrderId;
		}

		var publishMock = new Mock<IPublishEndpoint>();
		var userServiceMock = new Mock<IUserService>();
		userServiceMock.Setup(u => u.GetCurrentUserAsync()).ReturnsAsync(new ApplicationUser { Id = "man2", CompanyId = 21 });
		userServiceMock.Setup(u => u.GetUserByIdAsync("cust3")).ReturnsAsync(new ApplicationUser { Id = "cust3", Balance = 10m }); // not enough

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var handler = new CompleteOrderCommandHandler(ctx, userServiceMock.Object, publishMock.Object);

			var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(new CompleteOrderCommand(orderId.ToString()), default));
			ex.Message.Should().Be("Top up the balance.");
		}
	}

	[Fact]
	public async Task CancelOrder_ShouldIncreaseProductQuantity_AndSetCanceledStatus()
	{
		var dbName = TestDbHelper.NewDbName("CancelOrderTestDb");
		var sp = TestDbHelper.CreateServiceProvider(dbName);

		int orderId;

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var company = new Company { CompanyId = 30, Title = "Comp3" };
			await ctx.Companies.AddAsync(company);

			var product = new Product { ProductId = 300, Company = company, CompanyId = company.CompanyId, Title = "P3", Price = 20m, Quantity = 5 };
			await ctx.Products.AddAsync(product);

			var order = new Order
			{
				Number = "ORD-300",
				CustomerId = "cust4",
				Company = company,
				Status = OrderStatus.Confirmed,
				OrderDate = DateTime.UtcNow,
				OrderItems = []
			};
			order.OrderItems.Add(new OrderItem { Product = product, ProductId = product.ProductId, Quantity = 2, ProductPrice = product.Price });
			await ctx.Orders.AddAsync(order);
			await ctx.SaveChangesAsync();
			orderId = order.OrderId;
		}

		var userServiceMock = new Mock<IUserService>();
		userServiceMock.Setup(u => u.GetCurrentUserAsync()).ReturnsAsync(new ApplicationUser { Id = "cust4" });

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var handler = new CancelOrderCommandHandler(ctx, userServiceMock.Object);

			await handler.Handle(new CancelOrderCommand(orderId), default);

			var orderInDb = await ctx.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);
			orderInDb!.Status.Should().Be(OrderStatus.Canceled);
			orderInDb.ReceiveKey.Should().Be(0);

			var productInDb = await ctx.Products.FirstOrDefaultAsync(p => p.ProductId == 300);
			productInDb!.Quantity.Should().Be(7); // 5 + 2
		}
	}

	[Fact]
	public async Task CancelOrder_ShouldThrow_WhenOrderInInvalidStatus()
	{
		var dbName = TestDbHelper.NewDbName("CancelOrderFailTestDb");
		var sp = TestDbHelper.CreateServiceProvider(dbName);

		int orderId;

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var company = new Company { CompanyId = 31, Title = "Comp4" };
			await ctx.Companies.AddAsync(company);

			var product = new Product { ProductId = 301, Company = company, CompanyId = company.CompanyId, Title = "P4", Price = 20m, Quantity = 5 };
			await ctx.Products.AddAsync(product);

			var order = new Order
			{
				Number = "ORD-301",
				CustomerId = "cust5",
				Company = company,
				Status = OrderStatus.Completed,
				OrderDate = DateTime.UtcNow,
				OrderItems = []
			};
			order.OrderItems.Add(new OrderItem { Product = product, ProductId = product.ProductId, Quantity = 1, ProductPrice = product.Price });
			await ctx.Orders.AddAsync(order);
			await ctx.SaveChangesAsync();
			orderId = order.OrderId;
		}

		var userServiceMock = new Mock<IUserService>();
		userServiceMock.Setup(u => u.GetCurrentUserAsync()).ReturnsAsync(new ApplicationUser { Id = "cust5" });

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var handler = new CancelOrderCommandHandler(ctx, userServiceMock.Object);

			var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(new CancelOrderCommand(orderId), default));
			ex.Message.Should().Be("Only confirmed or ready orders can be canceled.");
		}
	}
}
