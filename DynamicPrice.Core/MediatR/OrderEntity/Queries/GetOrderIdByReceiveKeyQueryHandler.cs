using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public class GetOrderIdByReceiveKeyQueryHandler(
	DynamicPriceCoreContext context,
	IUserService userService)
	: IRequestHandler<GetOrderIdByReceiveKeyQuery, int>
{
	public async Task<int> Handle(
		GetOrderIdByReceiveKeyQuery request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var orderId = await context.Orders
			.Where(o =>
				o.ReceiveKey.ToString() == request.ReceiveKey &&
				o.CompanyId == manager.CompanyId)
			.Select(o => o.OrderId)
			.FirstOrDefaultAsync(cancellationToken);

		return orderId == 0
			? throw new NotFoundException($"Order with receive key '{request.ReceiveKey}' not found. Key must be a 6-digit number.")
			: orderId;
	}
}
