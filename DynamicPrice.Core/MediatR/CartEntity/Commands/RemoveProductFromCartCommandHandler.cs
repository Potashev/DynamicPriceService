using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.CartEntity.Commands;

public class RemoveProductFromCartCommandHandler
	: IRequestHandler<RemoveProductFromCartCommand, CartViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public RemoveProductFromCartCommandHandler(
		DynamicPriceCoreContext context,
		IMapper mapper,
		IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<CartViewModel> Handle(
		RemoveProductFromCartCommand request,
		CancellationToken cancellationToken)
	{
		var customer = await _userService.GetRequiredCurrentUserAsync();

		var cartItem = await _context.CartItems
			.Where(ci => ci.ProductId.ToString() == request.ProductId
				&& ci.Cart.CustomerId == customer.Id)
			.Include(ci => ci.Cart)
				.ThenInclude(c => c.Company)
			.FirstOrDefaultAsync(cancellationToken);

		if (cartItem is null)
			throw new NotFoundException("Элемент корзины не найден!");

		cartItem.Quantity -= 1;

		if (cartItem.Quantity == 0)
			_context.CartItems.Remove(cartItem);

		await _context.SaveChangesAsync(cancellationToken);

		return _mapper.Map<CartViewModel>(cartItem.Cart);
	}
}
