using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public class CompleteOrderCommandHandler
	: IRequestHandler<CompleteOrderCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IUserService _userService;
	private readonly IPublishEndpoint _publishEndpoint;

	public CompleteOrderCommandHandler(
		DynamicPriceCoreContext context,
		IUserService userService,
		IPublishEndpoint publishEndpoint)
		=> (_context, _userService, _publishEndpoint) = (context, userService, publishEndpoint);

	public async Task<int> Handle(
		CompleteOrderCommand request,
		CancellationToken cancellationToken)
	{
		var manager = await _userService.GetRequiredCurrentUserAsync();

		var order = await _context.Orders
			.Where(o => o.OrderId.ToString() == request.OrderId && o.CompanyId == manager.CompanyId)
			.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.Product)
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException("Order not found.");

		if (order.Status is not OrderStatus.Ready)
			throw new BusinessException("Only ready for receive orders can be completed.");

		var customer = await _userService.GetUserByIdAsync(order.CustomerId)
			?? throw new NotFoundException("Customer not found.");

		var orderTotalAmount = order.OrderItems
			.Sum(oi => oi.Quantity * oi.ProductPrice);

		if (customer.Balance < orderTotalAmount)
			throw new BusinessException("Top up the balance.");

		customer.Balance -= orderTotalAmount;

		order.MarkAsCompleted();

		await _context.SaveChangesAsync(cancellationToken);
		await _userService.UpdateUserAsync(customer);

		foreach (var item in order.OrderItems)
			await _publishEndpoint.Publish(
				new PriceIncreaseEvent(item.ProductId, item.Quantity),
				cancellationToken);

		return order.OrderId;
	}
}
