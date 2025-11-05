using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using DynamicPriceCore.MediatR.ProductEntity.Commands;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using DynamicPriceCore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Tests.MediatR.ProductEntity.Commands;

public class CreateProductCommandHandlerTests
{
	private readonly IMapper _mapper;

	public CreateProductCommandHandlerTests()
	{
		var config = new MapperConfiguration(cfg =>
		{
			cfg.CreateMap<ProductViewModel, Product>();
		});
		_mapper = config.CreateMapper();
	}

	[Fact]
	public async Task Handle_ShouldCreateProduct_AndAssignCompanyId_AndSetLastSellTime()
	{
		// Arrange
		var options = new DbContextOptionsBuilder<DynamicPriceCoreContext>()
			.UseInMemoryDatabase(databaseName: "CreateProductHandlerTestDb")
			.Options;

		await using var context = new DynamicPriceCoreContext(options);

		var userServiceMock = new Mock<IUserService>();
		userServiceMock
			.Setup(s => s.GetCurrentUserAsync())
			.ReturnsAsync(new ApplicationUser { CompanyId = 42 });

		var handler = new CreateProductCommandHandler(context, _mapper, userServiceMock.Object);

		var productVm = new ProductViewModel
		{
			Title = "My test product",
			Price = 1350.5m
		};

		var command = new CreateProductCommand(productVm);

		// Act
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		result.Should().BeGreaterThan(0);

		var productInDb = await context.Products.FirstOrDefaultAsync(p => p.ProductId == result);
		productInDb.Should().NotBeNull();
		productInDb!.Title.Should().Be("My test product");
		productInDb!.Price.Should().Be(1350.5m);
		productInDb.CompanyId.Should().Be(42);
		productInDb.LastSellTime.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(1));
	}
}
