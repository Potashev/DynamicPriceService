using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public class ReadyForReceiveOrderCommandHandler(
	DynamicPriceCoreContext context,
	IUserService userService)
	: IRequestHandler<ReadyForReceiveOrderCommand>
{
	public async Task Handle(
		ReadyForReceiveOrderCommand request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var order = await context.Orders
			.Where(o => o.Id == request.OrderId && o.CompanyId == manager.CompanyId)
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException("Order not found.");

		order.MarkAsReady();

		await context.SaveChangesAsync(cancellationToken);
		return;
	}
}

