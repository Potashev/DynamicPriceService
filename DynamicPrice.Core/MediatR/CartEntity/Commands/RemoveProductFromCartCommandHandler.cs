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

		var cart = await _context.Carts
			.Where(c => c.CustomerId == customer.Id
				&& c.CartItems.Any(ci => ci.ProductId == request.ProductId))
			.Include(c => c.CartItems)
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException("Cart not found");

		cart.RemoveItem(request.ProductId);

		await _context.SaveChangesAsync(cancellationToken);

		return _mapper.Map<CartViewModel>(cart);
	}
}
