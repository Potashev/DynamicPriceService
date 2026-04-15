using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.CustomerEntity.Queries;

public class GetCustomerInfoQueryHandler(
	DynamicPriceCoreContext context,
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<GetCustomerInfoQuery, CustomerInfoViewModel>
{
	public async Task<CustomerInfoViewModel> Handle(
		GetCustomerInfoQuery request,
		CancellationToken cancellationToken)
	{
		var customer = await userService.GetRequiredCurrentUserAsync();

		var customerOrders = await context.Orders
			.Include(o => o.Company)
			.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.Product)
			.Where(o => o.CustomerId == customer.Id)
			.OrderByDescending(o => o.OrderDate)
			.ToArrayAsync(cancellationToken);

		return new CustomerInfoViewModel
		{
			Name = customer.UserName ?? string.Empty,
			Email = customer.Email ?? string.Empty,
			Balance = customer.Balance,
			Orders = mapper.Map<OrderViewModel[]>(customerOrders)
		};
	}
}
