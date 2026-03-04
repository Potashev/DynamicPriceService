using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public class ConfirmOrderCommandHandler
	: IRequestHandler<ConfirmOrderCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IUserService _userService;

	public ConfirmOrderCommandHandler(
		DynamicPriceCoreContext context,
		IUserService userService)
		=> (_context, _userService) = (context, userService);

	public async Task<int> Handle(
		ConfirmOrderCommand request,
		CancellationToken cancellationToken)
	{
		var customer = await _userService.GetRequiredCurrentUserAsync();

		var cart = await _context.Carts
			.Where(c => c.CartId == request.CartId && c.CustomerId == customer.Id)
			.Include(c => c.Company)
			.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException("Cart not found.");

		var order = new Order(customer.Id, cart.Company);

		order.AddItems(cart.CartItems);

		_context.Orders.Add(order);
		_context.Carts.Remove(cart);

		await _context.SaveChangesAsync(cancellationToken);

		return order.OrderId;
	}
}
