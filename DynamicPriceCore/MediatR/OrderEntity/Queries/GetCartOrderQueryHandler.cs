using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetCartOrderQueryHandler
	: IRequestHandler<GetCartOrderQuery, Order>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly ICurrentUserService _currentUserService;

	public GetCartOrderQueryHandler(DynamicPriceCoreContext context, IMapper mapper, ICurrentUserService currentUserService)
		=> (_context, _mapper, _currentUserService) = (context, mapper, currentUserService);

	public async Task<Order> Handle(GetCartOrderQuery request, CancellationToken cancellationToken)
	{
		//var cartOrder = await _context.Orders
		//	.Include(o => o.Company)
		//	.Include(o => o.OrderProducts)
		//		.ThenInclude(op => op.Product)
		//	.FirstOrDefaultAsync(o => o.Customer.Id == request.CustomerId.ToString()   //todo: make string request.CustomerId
		//		&& o.Company.CompanyId == request.CompanyId 
		//		&& o.Status == OrderStatus.Cart, cancellationToken);


		var customer = await _currentUserService.GetCurrentCustomerAsync();

		var cartOrder = await _context.Orders
			.Include(o => o.Company)
			.Include(o => o.OrderProducts)
				.ThenInclude(op => op.Product)
			.FirstOrDefaultAsync(o => o.Customer.Id == customer.Id   //todo: make string request.CustomerId
				&& o.Company.CompanyId == request.CompanyId
				&& o.Status == OrderStatus.Cart, cancellationToken);


		return cartOrder;
	}
}
