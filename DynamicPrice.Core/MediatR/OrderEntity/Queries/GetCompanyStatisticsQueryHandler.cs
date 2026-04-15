using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public class GetCompanyStatisticsQueryHandler(
	DynamicPriceCoreContext context,
	IUserService userService)
	: IRequestHandler<GetCompanyStatisticsQuery, OrdersStatistics>
{
	public async Task<OrdersStatistics> Handle(
		GetCompanyStatisticsQuery request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var companyOrdersWithAmount = await context.Orders
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
