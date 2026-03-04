using AutoMapper;
using Bogus.DataSets;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.CartEntity.Commands;

public class AddProductToCartCommandHadnler
	: IRequestHandler<AddProductToCartCommand, CartViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public AddProductToCartCommandHadnler(
		DynamicPriceCoreContext context,
		IMapper mapper,
		IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<CartViewModel> Handle(
		AddProductToCartCommand request,
		CancellationToken cancellationToken)
	{
		var customer = await _userService.GetRequiredCurrentUserAsync();

		var product = await _context.Products
			.Include(p => p.Company)
			.Where(p => p.ProductId.ToString() == request.ProductId)
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException("Product not found");

		var cart = await _context.Carts
			.Include(c => c.CartItems)
			.Where(c => c.CustomerId == customer.Id
				&& c.Company == product.Company)
			.FirstOrDefaultAsync(cancellationToken);

		if (cart is null)
		{
			cart = new Cart(customer.Id, product.Company);
			await _context.Carts.AddAsync(cart);
		}

		cart.AddItem(product.ProductId);

		await _context.SaveChangesAsync(cancellationToken);
		return _mapper.Map<CartViewModel>(cart);
	}
}
