using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using DynamicPriceCore.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetCompanyStatisticsQueryHandler
	: IRequestHandler<GetCompanyStatisticsQuery, OrdersStatistics>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly ICurrentUserService _currentUserService;

	public GetCompanyStatisticsQueryHandler(DynamicPriceCoreContext context, ICurrentUserService currentUserService)
		=> (_context, _currentUserService) = (context, currentUserService);

	public async Task<OrdersStatistics> Handle(GetCompanyStatisticsQuery request, CancellationToken cancellationToken)
	{
		var manager = await _currentUserService.GetCurrentUserAsync();

		var companyOrdersWithAmount = await _context.Orders
			.Where(o => o.Company.CompanyId == manager.CompanyId)
			.Select(o => new
			{
				Order = o,
				OrderAmount = o.OrderItems.Sum(op => op.ProductPrice * op.Quantity)
			})
			.ToArrayAsync(cancellationToken);


		var orderStatistics = new OrdersStatistics();
		orderStatistics.OrdersQuantity = companyOrdersWithAmount.Length;
		orderStatistics.TotalAmount = companyOrdersWithAmount.Sum(o => o.OrderAmount);
		orderStatistics.AverageOrderAmount = companyOrdersWithAmount.Average(o => o.OrderAmount);

		return orderStatistics;
	}
}
