using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public class ConfirmOrderCommandHandler(
	DynamicPriceCoreContext context,
	IUserService userService)
	: IRequestHandler<ConfirmOrderCommand, Guid>
{
	public async Task<Guid> Handle(
		ConfirmOrderCommand request,
		CancellationToken cancellationToken)
	{
		var customer = await userService.GetRequiredCurrentUserAsync();

		var cart = await context.Carts
			.Where(c => c.Id == request.CartId && c.CustomerId == customer.Id)
			.Include(c => c.Company)
			.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException("Cart not found.");

		var order = new Order(customer.Id, cart.CompanyId);

		order.AddItems(cart.CartItems);

		context.Orders.Add(order);
		context.Carts.Remove(cart);

		await context.SaveChangesAsync(cancellationToken);

		return order.Id;
	}
}
