using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetCompanyOrderDetailsQueryHandler
	: IRequestHandler<GetCompanyOrderDetailsQuery, OrderViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public GetCompanyOrderDetailsQueryHandler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task<OrderViewModel> Handle(GetCompanyOrderDetailsQuery request, CancellationToken cancellationToken)
	{
		var companyOrder = await _context.Orders
			.Where(o => o.OrderId.ToString() == request.OrderId) // perfomance - convert request to int?
			.Include(o => o.OrderProducts)
			.ThenInclude(op => op.Product)
			.FirstOrDefaultAsync(cancellationToken);

		var companyOrderVm = _mapper.Map<OrderViewModel>(companyOrder);

		companyOrderVm.OrderAmount = GetOrderPrice(companyOrderVm);

		return companyOrderVm;
	}

	private double GetOrderPrice(OrderViewModel orderVm)
	{
		double sum = 0;
		foreach (var orderProduct in orderVm.OrderProducts)
		{
			sum += (double)(orderProduct.Price * orderProduct.Quantity);
		}
		return sum;
	}
}
