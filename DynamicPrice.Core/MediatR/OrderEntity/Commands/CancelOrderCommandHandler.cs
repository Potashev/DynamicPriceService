using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public class CancelOrderCommandHandler(
	DynamicPriceCoreContext context,
	IUserService userService)
	: IRequestHandler<CancelOrderCommand, Guid>
{
	public async Task<Guid> Handle(
		CancelOrderCommand request,
		CancellationToken cancellationToken)
	{
		var customer = await userService.GetRequiredCurrentUserAsync();

		var order = await context.Orders
			.Where(o => o.Id == request.OrderId && o.CustomerId == customer.Id)
			.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.Product)
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException("Order not found.");

		order.MarkAsCanceled();

		await context.SaveChangesAsync(cancellationToken);
		return request.OrderId;
	}
}
