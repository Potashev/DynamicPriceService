using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class RemoveProductFromOrderCommandHandler
	: IRequestHandler<RemoveProductFromOrderCommand>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public RemoveProductFromOrderCommandHandler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task Handle(RemoveProductFromOrderCommand request, CancellationToken cancellationToken)
	{
		//var product = await _context.Products
		//	.Include(p => p.Company)
		//	.Where(p => p.ProductId.ToString() == request.ProductId)
		//	.FirstOrDefaultAsync();

		//var cartOrder = await _context.Orders
		//	.Include(o => o.OrderProducts)
		//	.Where(o => o.Customer.CustomerId.ToString() == request.CustomerId
		//		&& o.Company == product.Company
		//		&& o.Status == OrderStatus.Cart)
		//	.FirstOrDefaultAsync();


		//var orderproduct = cartOrder.OrderProducts
		//	.Where(op => op.ProductId == product.ProductId)
		//	.FirstOrDefault();

		//if (orderproduct == null)
		//{
		//	//todo: make better
		//	orderproduct = new OrderProduct
		//	{
		//		Order = cartOrder,
		//		Product = product,
		//		//Price		= product.Price,
		//		Quantity = 1
		//	};
		//	cartOrder.OrderProducts.Add(orderproduct);
		//}
		//else
		//{
		//	orderproduct.Quantity += 1;
		//}
		//_context.SaveChanges();
		//return cartOrder;

		var orderproduct = await _context.OrderProducts
			.Where(o => o.ProductId.ToString() == request.ProductId &&
			o.Order.Customer.CustomerId.ToString() == request.CustomerId)
			.FirstOrDefaultAsync();

		orderproduct.Quantity -= 1;

		if (orderproduct.Quantity == 0)
			_context.OrderProducts.Remove(orderproduct);

		await _context.SaveChangesAsync();
	}
}
