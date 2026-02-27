using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.MediatR.CartEntity.Queries;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using DynamicPrice.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DynamicPrice.Tests.MediatR.CartEntity;

public class GetCartDetailsQueryHandlerTests
{
	[Fact]
	public async Task Handle_ShouldReturnMappedCart_ForCurrentUserAndCompany()
	{
		var dbName = TestDbHelper.NewDbName("GetCartDetailsTestDb");
		var sp = TestDbHelper.CreateServiceProvider(dbName);

		var customerId = Guid.NewGuid().ToString();

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var company = new Company { CompanyId = 5, Title = "Co" };
			await ctx.Companies.AddAsync(company);
			var product = new Product { ProductId = 50, Company = company, CompanyId = company.CompanyId, Title = "Prod", Price = 9m };
			await ctx.Products.AddAsync(product);

			var cart = new Cart { CustomerId = customerId, Company = company, CartItems = [] };
			cart.CartItems.Add(new CartItem { Product = product, ProductId = product.ProductId, Quantity = 1, Cart = cart });
			await ctx.Carts.AddAsync(cart);
			await ctx.SaveChangesAsync();
		}

		var mapperMock = new Mock<IMapper>();
		mapperMock.Setup(m => m.Map<CartViewModel>(It.IsAny<Cart>()))
			.Returns((Cart c) => new CartViewModel
			{
				CartId = c.CartId,
				Company = new CompanyViewModel { CompanyId = c.Company.CompanyId, Title = c.Company.Title },
				CartItems = c.CartItems?.Select(ci => new CartItemViewModel { Id = ci.Id, CartId = ci.CartId, ProductId = ci.ProductId, Quantity = ci.Quantity, Product = new ProductViewModel { ProductId = ci.Product.ProductId, Title = ci.Product.Title, Price = ci.Product.Price } }).ToList() ?? new List<CartItemViewModel>()
			});

		var userServiceMock = new Mock<IUserService>();
		userServiceMock.Setup(u => u.GetRequiredCurrentUserAsync()).ReturnsAsync(new ApplicationUser { Id = customerId });

		using (var scope = sp.CreateScope())
		{
			var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
			var handler = new GetCartDetailsQueryHandler(ctx, mapperMock.Object, userServiceMock.Object);
			var result = await handler.Handle(new GetCartDetailsQuery(5), default);
			result.Should().NotBeNull();
			result.Company.CompanyId.Should().Be(5);
			result.CartItems.Should().HaveCount(1);
		}
	}

	[Fact]
	public async Task Handle_ShouldReturnNull_WhenCartNotFound()
	{
		var dbName = TestDbHelper.NewDbName("GetCartDetailsEmptyTestDb");
		var sp = TestDbHelper.CreateServiceProvider(dbName);

		var mapperMock = new Mock<IMapper>();
		mapperMock.Setup(m => m.Map<CartViewModel>(It.IsAny<Cart>())).Returns((CartViewModel)null!);

		var customerId = Guid.NewGuid().ToString();

		var userServiceMock = new Mock<IUserService>();
		userServiceMock.Setup(u => u.GetRequiredCurrentUserAsync()).ReturnsAsync(new ApplicationUser { Id = customerId });

		using var scope = sp.CreateScope();
		var ctx = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
		var handler = new GetCartDetailsQueryHandler(ctx, mapperMock.Object, userServiceMock.Object);
		var result = await handler.Handle(new GetCartDetailsQuery(999), default);
		result.Should().BeNull();
	}
}
