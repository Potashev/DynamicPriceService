using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetCartOrderQueryHandler
	: IRequestHandler<GetCartOrderQuery, Order>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public GetCartOrderQueryHandler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task<Order> Handle(GetCartOrderQuery request, CancellationToken cancellationToken)
	{
		var cartOrder = await _context.Orders
			.Include(o => o.Company)
			.Include(o => o.OrderProducts)
				.ThenInclude(op => op.Product)
			.FirstOrDefaultAsync(o => o.Customer.CustomerId == request.CustomerId 
				&& o.Company.CompanyId == request.CompanyId 
				&& o.Status == OrderStatus.Cart, cancellationToken);
		return cartOrder;
	}
}
