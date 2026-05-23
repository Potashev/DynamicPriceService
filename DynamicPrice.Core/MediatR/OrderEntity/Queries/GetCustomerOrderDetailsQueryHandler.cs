using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public class GetCustomerOrderDetailsQueryHandler(
	DynamicPriceCoreContext context,
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<GetCustomerOrderDetailsQuery, OrderViewModel>
{
	public async Task<OrderViewModel> Handle(
		GetCustomerOrderDetailsQuery request,
		CancellationToken cancellationToken)
	{
		var customer = await userService.GetRequiredCurrentUserAsync();

		var customerOrder = await context.Orders
			.Where(o =>
				o.Id == request.OrderId &&
				o.CustomerId == customer.Id)
			.Include(o => o.OrderItems)
				.ThenInclude(op => op.Product)
			.Include(o => o.Company)
			.FirstOrDefaultAsync(cancellationToken);

		return mapper.Map<OrderViewModel>(customerOrder);
	}
}
