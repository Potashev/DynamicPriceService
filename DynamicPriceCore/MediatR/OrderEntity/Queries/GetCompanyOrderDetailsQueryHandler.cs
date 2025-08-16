using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using DynamicPriceCore.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetCompanyOrderDetailsQueryHandler
	: IRequestHandler<GetCompanyOrderDetailsQuery, OrderViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly ICurrentUserService _currentUserService;

	public GetCompanyOrderDetailsQueryHandler(DynamicPriceCoreContext context, IMapper mapper, ICurrentUserService currentUserService)
		=> (_context, _mapper, _currentUserService) = (context, mapper, currentUserService);

	public async Task<OrderViewModel> Handle(GetCompanyOrderDetailsQuery request, CancellationToken cancellationToken)
	{
		var manager = await _currentUserService.GetCurrentUserAsync();

		var companyOrder = await _context.Orders
			.Where(o => o.OrderId.ToString() == request.OrderId && o.Company.CompanyId == manager.CompanyId) //todo: check and perfomance - convert request to int?
			.Include(o => o.OrderItems)
				.ThenInclude(op => op.Product)
			.FirstOrDefaultAsync(cancellationToken);

		var companyOrderVm = _mapper.Map<OrderViewModel>(companyOrder);

		companyOrderVm.OrderAmount = GetOrderPrice(companyOrderVm);

		return companyOrderVm;
	}

	private double GetOrderPrice(OrderViewModel orderVm)
	{
		double sum = 0;
		foreach (var orderItem in orderVm.OrderItems)
		{
			sum += (double)(orderItem.ProductPrice * orderItem.Quantity);
		}
		return sum;
	}
}
