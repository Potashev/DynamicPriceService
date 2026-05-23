using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public class GetCompanyOrderDetailsQueryHandler(
	DynamicPriceCoreContext context,
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<GetCompanyOrderDetailsQuery, OrderViewModel>
{
	public async Task<OrderViewModel> Handle(
		GetCompanyOrderDetailsQuery request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var companyOrder = await context.Orders
			.Where(o => o.Id == request.OrderId && o.CompanyId == manager.CompanyId)
			.Include(o => o.OrderItems)
				.ThenInclude(op => op.Product)
			.FirstOrDefaultAsync(cancellationToken);

		var companyOrderVm = mapper.Map<OrderViewModel>(companyOrder);

		if (!VirtualCustomer.IsVirtualCustomer(companyOrderVm.CustomerId))
			companyOrderVm.CustomerName = (await userService.GetUserByIdAsync(companyOrderVm.CustomerId))?.UserName ?? string.Empty;

		return companyOrderVm;
	}
}
