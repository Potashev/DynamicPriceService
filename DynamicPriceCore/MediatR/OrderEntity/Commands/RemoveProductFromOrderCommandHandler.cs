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
		var orderproduct = await _context.OrderProducts
			.Where(o => o.ProductId.ToString() == request.ProductId &&
			o.Order.Customer.CustomerId.ToString() == request.CustomerId)
			.FirstOrDefaultAsync(cancellationToken);

		orderproduct.Quantity -= 1;

		if (orderproduct.Quantity == 0)
			_context.OrderProducts.Remove(orderproduct);

		await _context.SaveChangesAsync();
	}
}
