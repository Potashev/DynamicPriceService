using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public class GetCompanyOrdersQueryHandler(
	DynamicPriceCoreContext context,
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<GetCompanyOrdersQuery, IEnumerable<OrderViewModel>>
{
	public async Task<IEnumerable<OrderViewModel>> Handle(
		GetCompanyOrdersQuery request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var companyOrders = await context.Orders
			.Where(o => o.CompanyId == manager.CompanyId)
			.OrderByDescending(o => o.OrderDate)
			.ToArrayAsync(cancellationToken);

		var companyOrdersVm = mapper.Map<OrderViewModel[]>(companyOrders);

		foreach (var orderVm in companyOrdersVm.Where(o => !VirtualCustomer.IsVirtualCustomer(o.CustomerId)))
		{
			orderVm.CustomerName = (await userService.GetUserByIdAsync(orderVm.CustomerId))?.UserName ?? string.Empty;
		}

		return companyOrdersVm;
	}
}
