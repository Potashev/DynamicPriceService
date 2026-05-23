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
			.Where(p => p.Id.ToString() == request.ProductId)	//todo: check guid.tostring()
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException("Product not found");

		if (!product.IsActive())
			throw new BusinessException($"Product '{product.Title}' is not available now");

		var cart = await context.Carts
			.Include(c => c.CartItems)
			.Where(c => c.CustomerId == customer.Id
				&& c.Id == product.CompanyId)
			.FirstOrDefaultAsync(cancellationToken);

		if (cart is null)
		{
			cart = new Cart(customer.Id, product.CompanyId);
			await context.Carts.AddAsync(cart);
		}

		cart.AddItem(product.Id);

		await context.SaveChangesAsync(cancellationToken);
		return mapper.Map<CartViewModel>(cart);
	}
}
