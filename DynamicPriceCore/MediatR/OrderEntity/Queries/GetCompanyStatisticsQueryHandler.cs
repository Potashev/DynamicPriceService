using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetCompanyStatisticsQueryHandler
	: IRequestHandler<GetCompanyStatisticsQuery, OrderStatistics>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly ICurrentUserService _currentUserService;

	public GetCompanyStatisticsQueryHandler(DynamicPriceCoreContext context, IMapper mapper, ICurrentUserService currentUserService)
		=> (_context, _mapper, _currentUserService) = (context, mapper, currentUserService);

	public async Task<OrderStatistics> Handle(GetCompanyStatisticsQuery request, CancellationToken cancellationToken)
	{
		//var companyOrdersWithAmount = await _context.Orders
		//	.Where(o => o.Company.CompanyUsers.Any(cu => cu.UserId == request.UserId))
		//	.Select(o => new
		//	{
		//		Order = o,
		//		OrderAmount = o.OrderProducts.Sum(op => op.Price * op.Quantity)
		//	})
		//	.ToArrayAsync(cancellationToken);

		var manager = await _currentUserService.GetCurrentUserAsync();

		//todo: check
		var companyOrdersWithAmount = await _context.Orders
			.Where(o => o.Company.CompanyId == manager.CompanyId)
			.Select(o => new
			{
				Order = o,
				OrderAmount = o.OrderProducts.Sum(op => op.Price * op.Quantity)
			})
			.ToArrayAsync(cancellationToken);


		var orderStatistics = new OrderStatistics();
		orderStatistics.OrdersQuantity = companyOrdersWithAmount.Length;
		orderStatistics.TotalAmount = companyOrdersWithAmount.Sum(o => o.OrderAmount);
		orderStatistics.AverageOrderAmount = companyOrdersWithAmount.Average(o => o.OrderAmount);

		return orderStatistics;
	}
}
