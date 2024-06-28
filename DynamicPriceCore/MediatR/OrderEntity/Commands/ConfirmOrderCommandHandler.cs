using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class ConfirmOrderCommandHandler
	: IRequestHandler<ConfirmOrderCommand, double>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IIncreasePriceService _increasePriceService;

	public ConfirmOrderCommandHandler(DynamicPriceCoreContext context, IIncreasePriceService increasePriceService)
		=> (_context, _increasePriceService) = (context, increasePriceService);

	public async Task<double> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
	{
		var order = await _context.Orders
			.Include(o => o.OrderProducts)
				.ThenInclude(op => op.Product)
			.Where(o => o.OrderId == request.CastOrderId)
			.FirstOrDefaultAsync();

		var orderPrice = SetOrderPrice(order);
		_increasePriceService.Increase(order.OrderProducts.Select(op => op.Product.ProductId));

		foreach (var op in order.OrderProducts)
		{
			var product = op.Product;
			product.LastSellTime = DateTime.UtcNow;
			if (product.Quantity != null)
				product.Quantity -= op.Quantity;
		}
		order.Status = OrderStatus.Confirmed;
		order.OrderDate = DateTime.UtcNow;

		_context.SaveChanges();
		return orderPrice;
	}

	private double SetOrderPrice(Order order)
	{
		double sum = 0;
		foreach (var orderProduct in order.OrderProducts)
		{
			if (orderProduct.Price == null)
				orderProduct.Price = orderProduct.Product.Price;

			sum += (double)(orderProduct.Price * orderProduct.Quantity);
		}
		return sum;
	}
}
