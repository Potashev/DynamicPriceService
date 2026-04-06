using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public class GetCompanyStatisticsQueryHandler
	: IRequestHandler<GetCompanyStatisticsQuery, OrdersStatistics>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IUserService _userService;

	public GetCompanyStatisticsQueryHandler(
		DynamicPriceCoreContext context,
		IUserService userService)
		=> (_context, _userService) = (context, userService);

	public async Task<OrdersStatistics> Handle(
		GetCompanyStatisticsQuery request,
		CancellationToken cancellationToken)
	{
		throw new NotFoundException("NotFoundException!");

		var manager = await _userService.GetRequiredCurrentUserAsync();

		var companyOrdersWithAmount = await _context.Orders
			.Where(o => o.CompanyId == manager.CompanyId)
			.Select(o => new
			{
				Order = o,
				OrderAmount = o.OrderItems.Sum(op => op.ProductPrice * op.Quantity)
			})
			.ToArrayAsync(cancellationToken);

		return new OrdersStatistics
		{
			OrdersQuantity = companyOrdersWithAmount.Length,
			TotalAmount = companyOrdersWithAmount.Sum(o => o.OrderAmount),
			AverageOrderTotal = companyOrdersWithAmount.Average(o => o.OrderAmount)
		};
	}
}
