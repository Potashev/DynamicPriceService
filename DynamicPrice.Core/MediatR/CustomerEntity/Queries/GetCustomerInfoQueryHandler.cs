using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.CustomerEntity.Queries;

public class GetCustomerInfoQueryHandler
	: IRequestHandler<GetCustomerInfoQuery, CustomerInfoViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public GetCustomerInfoQueryHandler(
		DynamicPriceCoreContext context,
		IMapper mapper,
		IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<CustomerInfoViewModel> Handle(
		GetCustomerInfoQuery request,
		CancellationToken cancellationToken)
	{
		var customer = await _userService.GetRequiredCurrentUserAsync();

		var customerOrders = await _context.Orders
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
			Orders = _mapper.Map<OrderViewModel[]>(customerOrders)
		};
	}
}
