using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public class CancelOrderCommandHandler
	: IRequestHandler<CancelOrderCommand>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IUserService _userService;

	// todo: di
	public CancelOrderCommandHandler(
		DynamicPriceCoreContext context,
		IUserService userService)
		=> (_context, _userService) = (context, userService);

	public async Task Handle(
		CancelOrderCommand request,
		CancellationToken cancellationToken)
	{
		var customer = await _userService.GetRequiredCurrentUserAsync();

		var order = await _context.Orders
			.Where(o => o.OrderId == request.OrderId && o.CustomerId == customer.Id)
			.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.Product)
			.FirstOrDefaultAsync(cancellationToken);

		if (order is null)
			throw new Exception("Order not found.");

		if (order.Status is OrderStatus.Canceled or OrderStatus.Completed)
			throw new Exception("Only confirmed or ready orders can be canceled.");

		foreach (var oi in order.OrderItems)
		{
			if (oi.Product.Quantity != null)
				oi.Product.Quantity += oi.Quantity;
		}

		order.Status = OrderStatus.Canceled;
		order.ReceiveKey = 0;

		await _context.SaveChangesAsync(cancellationToken);

		return;
	}
}
