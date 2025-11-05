using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using DynamicPriceCore.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.CartEntity.Commands;

public class RemoveProductFromCartCommandHandler
	: IRequestHandler<RemoveProductFromCartCommand, CartViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public RemoveProductFromCartCommandHandler(DynamicPriceCoreContext context, IMapper mapper, IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<CartViewModel> Handle(RemoveProductFromCartCommand request, CancellationToken cancellationToken)
	{
		var customer = await _userService.GetCurrentUserAsync();

		var cartItem = await _context.CartItems
			.Where(ci => ci.ProductId.ToString() == request.ProductId
				&& ci.Cart.CustomerId == customer.Id)
			.Include(ci => ci.Cart)
				.ThenInclude(c => c.Company)
			.FirstOrDefaultAsync(cancellationToken);

		cartItem.Quantity -= 1;

		if (cartItem.Quantity == 0)
			_context.CartItems.Remove(cartItem);

		await _context.SaveChangesAsync(cancellationToken);

		return _mapper.Map<CartViewModel>(cartItem.Cart);
	}
}
