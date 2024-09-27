using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class RemoveProductFromOrderCommandHandler
	: IRequestHandler<RemoveProductFromOrderCommand, Order>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public RemoveProductFromOrderCommandHandler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task<Order> Handle(RemoveProductFromOrderCommand request, CancellationToken cancellationToken)
	{
		var orderproduct = await _context.OrderProducts
			.Where(op => op.ProductId.ToString() == request.ProductId
				&& op.Order.Customer.CustomerId.ToString() == request.CustomerId
				&& op.Order.Status == OrderStatus.Cart)
			.Include(op => op.Order)
				.ThenInclude(o => o.Company)
			.FirstOrDefaultAsync(cancellationToken);

		orderproduct.Quantity -= 1;

		if (orderproduct.Quantity == 0)
			_context.OrderProducts.Remove(orderproduct);

		await _context.SaveChangesAsync(cancellationToken);

		return orderproduct.Order;
	}
}
