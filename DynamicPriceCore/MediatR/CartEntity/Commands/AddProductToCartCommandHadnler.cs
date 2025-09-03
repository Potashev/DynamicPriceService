using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using DynamicPriceCore.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.CartEntity.Commands;

public class AddProductToCartCommandHadnler
	: IRequestHandler<AddProductToCartCommand, CartViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public AddProductToCartCommandHadnler(DynamicPriceCoreContext context, IMapper mapper, IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<CartViewModel> Handle(AddProductToCartCommand request, CancellationToken cancellationToken)
	{
		var customer = await _userService.GetCurrentUserAsync();

		var product = await _context.Products
			.Include(p => p.Company)
			.Where(p => p.ProductId.ToString() == request.ProductId)
			.FirstOrDefaultAsync(cancellationToken);

		var cart = await _context.Carts
			.Include(c => c.CartItems)
			.Where(c => c.CustomerId == customer.Id
				&& c.Company == product.Company)
			.FirstOrDefaultAsync(cancellationToken);

		if (cart == null)
			cart = await CreateNewCart(customer, product.Company);

		var cartItem = cart.CartItems
			.Where(ci => ci.ProductId == product.ProductId)
			.FirstOrDefault();

		if (cartItem == null)
		{
			//todo: make better
			cartItem = new CartItem
			{
				Cart = cart,
				Product = product,
				Quantity = 1
			};
			cart.CartItems.Add(cartItem);
		}
		else
		{
			cartItem.Quantity += 1;
		}

		await _context.SaveChangesAsync(cancellationToken);
		return _mapper.Map<CartViewModel>(cart);
	}

	private async Task<Cart> CreateNewCart(ApplicationUser customer, Company company)
	{
		var cart = new Cart
		{
			CustomerId = customer.Id,
			Company = company,
			CartItems = new List<CartItem>()	//is it right?
		};

		await _context.Carts.AddAsync(cart);
		await _context.SaveChangesAsync();
		return cart;
	}
}
