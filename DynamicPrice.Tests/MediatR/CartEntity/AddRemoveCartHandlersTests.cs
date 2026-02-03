using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.MediatR.CartEntity.Commands;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using DynamicPrice.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DynamicPrice.Tests.MediatR.CartEntity;

public class AddRemoveCartHandlersTests
{
	[Fact]
	public async Task AddProductToCart_ShouldCreateCartAndAddItem_OrIncreaseQuantity()
	{
		var dbName = TestDbHelper.NewDbName("AddRemoveCartTestDb");
		var sp = TestDbHelper.CreateServiceProvider(dbName);

		var productId = 10;

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var company = new Company { CompanyId = 1, Title = "C1" };
			await ctx.Companies.AddAsync(company);
			var product = new Product { ProductId = productId, Company = company, CompanyId = company.CompanyId, Title = "P1", Price = 5m };
			await ctx.Products.AddAsync(product);
			await ctx.SaveChangesAsync();
		}

		// Create mapper mock that maps entities to view models (null-safe)
		var mapperMock = new Mock<IMapper>();
		mapperMock.Setup(m => m.Map<CartViewModel>(It.IsAny<Cart>()))
			.Returns((Cart c) => new CartViewModel
			{
				CartId = c.CartId,
				Company = new CompanyViewModel { CompanyId = c.Company.CompanyId, Title = c.Company.Title },
				CartItems = c.CartItems?.Select(ci => new CartItemViewModel { Id = ci.Id, CartId = ci.CartId, ProductId = ci.ProductId, Product = new ProductViewModel { ProductId = ci.Product != null ? ci.Product.ProductId : ci.ProductId, Title = ci.Product?.Title, Price = ci.Product?.Price ?? default, MinimumPrice = ci.Product?.MinimumPrice ?? default, Quantity = ci.Product?.Quantity }, Quantity = ci.Quantity }).ToList()
			});

		// first call - new cart
		var userServiceMock = new Mock<IUserService>();
		userServiceMock.Setup(u => u.GetRequiredCurrentUserAsync()).ReturnsAsync(new ApplicationUser { Id = Guid.NewGuid().ToString() });

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var handler = new AddProductToCartCommandHadnler(ctx, mapperMock.Object, userServiceMock.Object);
			var result = await handler.Handle(new AddProductToCartCommand(productId.ToString()), default);
			result.Should().NotBeNull();
			result.CartItems.Should().HaveCount(1);
			result.Company.CompanyId.Should().Be(1);
		}

		// second call - increase quantity
		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var handler = new AddProductToCartCommandHadnler(ctx, mapperMock.Object, userServiceMock.Object);
			var result = await handler.Handle(new AddProductToCartCommand(productId.ToString()), default);
			result.CartItems.Should().HaveCount(1);
			result.CartItems.First().Quantity.Should().Be(2);
		}
	}

	[Fact]
	public async Task RemoveProductFromCart_ShouldDecreaseOrRemoveItem()
	{
		var dbName = TestDbHelper.NewDbName("AddRemoveCartTestDb");
		var sp = TestDbHelper.CreateServiceProvider(dbName);

		var productId = 20;
		var customerId = Guid.NewGuid().ToString();

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var company = new Company { CompanyId = 2, Title = "C2" };
			await ctx.Companies.AddAsync(company);
			var product = new Product { ProductId = productId, Company = company, CompanyId = company.CompanyId, Title = "P2", Price = 7m };
			await ctx.Products.AddAsync(product);
			var cart = new Cart { CustomerId = customerId, Company = company, CartItems = [] };
			cart.CartItems.Add(new CartItem { Product = product, ProductId = product.ProductId, Quantity = 2, Cart = cart });
			await ctx.Carts.AddAsync(cart);
			await ctx.SaveChangesAsync();
		}

		var mapperMock = new Mock<IMapper>();
		mapperMock.Setup(m => m.Map<CartViewModel>(It.IsAny<Cart>()))
			.Returns((Cart c) => new CartViewModel
			{
				CartId = c.CartId,
				Company = new CompanyViewModel { CompanyId = c.Company.CompanyId, Title = c.Company.Title },
				CartItems = c.CartItems?.Select(ci => new CartItemViewModel { Id = ci.Id, CartId = ci.CartId, ProductId = ci.ProductId, Product = new ProductViewModel { ProductId = ci.Product != null ? ci.Product.ProductId : ci.ProductId, Title = ci.Product?.Title, Price = ci.Product?.Price ?? default, MinimumPrice = ci.Product?.MinimumPrice ?? default, Quantity = ci.Product?.Quantity }, Quantity = ci.Quantity }).ToList()
			});

		var userServiceMock = new Mock<IUserService>();
		userServiceMock.Setup(u => u.GetRequiredCurrentUserAsync()).ReturnsAsync(new ApplicationUser { Id = customerId });

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var handler = new RemoveProductFromCartCommandHandler(ctx, mapperMock.Object, userServiceMock.Object);
			var result = await handler.Handle(new RemoveProductFromCartCommand(productId.ToString()), default);
			result.CartItems.First().Quantity.Should().Be(1);
		}

		// remove last
		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var handler = new RemoveProductFromCartCommandHandler(ctx, mapperMock.Object, userServiceMock.Object);
			var result = await handler.Handle(new RemoveProductFromCartCommand(productId.ToString()), default);
			result.CartItems.Should().BeEmpty();
		}
	}
}
