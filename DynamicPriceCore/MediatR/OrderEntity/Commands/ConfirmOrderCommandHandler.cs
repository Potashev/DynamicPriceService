using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class TopUpBalanceCommandHandler
	: IRequestHandler<ConfirmOrderCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IIncreasePriceService _increasePriceService;
	private readonly ICurrentUserService _currentUserService;

	public TopUpBalanceCommandHandler(DynamicPriceCoreContext context, IIncreasePriceService increasePriceService, ICurrentUserService currentUserService)
		=> (_context, _increasePriceService, _currentUserService) = (context, increasePriceService, currentUserService);

	public async Task<int> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
	{
		//todo: check that request from customer?

		var customer = await _currentUserService.GetCurrentUserAsync();


		//var order = await _context.Orders
		//	.Include(o => o.OrderProducts)
		//		.ThenInclude(op => op.Product)
		//	.Where(o => o.OrderId == request.CartOrderId)
		//	.FirstOrDefaultAsync(cancellationToken);

		//decimal orderTotalAmout = 0;

		//foreach (var orderProduct in order.OrderProducts)
		//{
		//	var product = orderProduct.Product;
		//	orderProduct.Price = product.Price;
		//	product.LastSellTime = DateTime.UtcNow;
		//	if (product.Quantity != null)
		//		product.Quantity -= orderProduct.Quantity;

		//	orderTotalAmout += orderProduct.Quantity * orderProduct.Price;
		//}

		//if(customer.Balance >= orderTotalAmout)
		//{
		//	customer.Balance -= orderTotalAmout;

		//	order.Status = OrderStatus.Confirmed;
		//	order.OrderDate = DateTime.UtcNow;
		//	order.ReceiveKey = GenerateReceiveKey();
		//}
		//else
		//{
		//	throw new Exception("Top up the balance!");
		//}

		//	await _context.SaveChangesAsync(cancellationToken);

		//await _increasePriceService.Increase(order.OrderProducts);

		//return order.ReceiveKey;
		return 1;
	}

	private int GenerateReceiveKey() => new Random().Next(100000, 1000000);
}
