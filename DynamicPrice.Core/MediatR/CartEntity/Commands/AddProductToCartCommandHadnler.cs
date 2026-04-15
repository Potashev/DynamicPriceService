using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.CartEntity.Commands;

public class AddProductToCartCommandHadnler(
	DynamicPriceCoreContext context,
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<AddProductToCartCommand, CartViewModel>
{
	public async Task<CartViewModel> Handle(
		AddProductToCartCommand request,
		CancellationToken cancellationToken)
	{
		var customer = await userService.GetRequiredCurrentUserAsync();

		var product = await context.Products
			.Include(p => p.Company)
			.Where(p => p.ProductId.ToString() == request.ProductId)
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException("Product not found");

		var cart = await context.Carts
			.Include(c => c.CartItems)
			.Where(c => c.CustomerId == customer.Id
				&& c.CompanyId == product.CompanyId)
			.FirstOrDefaultAsync(cancellationToken);

		if (cart is null)
		{
			cart = new Cart(customer.Id, product.CompanyId);
			await context.Carts.AddAsync(cart);
		}

		cart.AddItem(product.ProductId);

		await context.SaveChangesAsync(cancellationToken);
		return mapper.Map<CartViewModel>(cart);
	}
}
