using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class CompleteOrderCommandHandler
	: IRequestHandler<CompleteOrderCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IIncreasePriceService _increasePriceService;

	public CompleteOrderCommandHandler(DynamicPriceCoreContext context, IIncreasePriceService increasePriceService)
		=> (_context, _increasePriceService) = (context, increasePriceService);

	public async Task<int> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
	{
		var order = await _context.Orders
			.Where(o => o.OrderId.ToString() == request.OrderId)
			.FirstOrDefaultAsync(cancellationToken);

		order.Status = OrderStatus.Completed;
		order.ReceiveKey = 0;	//todo: think about nullable

		await _context.SaveChangesAsync(cancellationToken);

		return order.OrderId;
	}
}
