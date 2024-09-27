using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class ConfirmOrderCommandHandler
	: IRequestHandler<ConfirmOrderCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IIncreasePriceService _increasePriceService;

	public ConfirmOrderCommandHandler(DynamicPriceCoreContext context, IIncreasePriceService increasePriceService)
		=> (_context, _increasePriceService) = (context, increasePriceService);

	public async Task<int> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
	{
		var order = await _context.Orders
			.Include(o => o.OrderProducts)
				.ThenInclude(op => op.Product)
			.Where(o => o.OrderId == request.CartOrderId)
			.FirstOrDefaultAsync(cancellationToken);

		foreach (var orderProduct in order.OrderProducts)
		{
			var product = orderProduct.Product;
			orderProduct.Price = product.Price;
			product.LastSellTime = DateTime.UtcNow;
			if (product.Quantity != null)
				product.Quantity -= orderProduct.Quantity;
		}
		order.Status		= OrderStatus.Confirmed;
		order.OrderDate		= DateTime.UtcNow;
		order.ReceiveKey	= GenerateReceiveKey();

		await _context.SaveChangesAsync(cancellationToken);

		await _increasePriceService.Increase(order.OrderProducts);

		return order.ReceiveKey;
	}

	private int GenerateReceiveKey() => new Random().Next(100000, 1000000);
}
