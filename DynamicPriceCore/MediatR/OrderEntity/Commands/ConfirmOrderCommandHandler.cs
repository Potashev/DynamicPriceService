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
			.Include(o => o.Products)
			.Where(o => o.OrderId == request.CastOrderId)
			.FirstOrDefaultAsync();

		// фиксируем цену продуктов для заказа и итоговую сумму заказа (пока просто храним)
		var orderPrice = GetOrderPrice(order);
		// повышаем цену продуктам 
		_increasePriceService.Increase(order.Products);
		// продуктам обновляем дату продажи и уменьшаем quantity и продуктов в компании (где есть)
		foreach(var product in order.Products)
		{
			product.LastSellTime = DateTime.UtcNow;
			if (product.Quantity != null)
				product.Quantity -= 1;
		}
		// меняем статус у заказа и добавляем дату
		order.Status = OrderStatus.Confirmed;
		order.OrderDate = DateTime.UtcNow;

		_context.SaveChanges();
		return orderPrice;
	}

	private double GetOrderPrice(Order order)
	{
		double sum = 0;
		foreach (var product in order.Products) 
			sum += product.Price;
		return sum;
	}
}
