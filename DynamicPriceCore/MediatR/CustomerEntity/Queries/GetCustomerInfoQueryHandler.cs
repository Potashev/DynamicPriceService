using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.CustomerEntity.Queries;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using DynamicPriceCore.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderStatus = DynamicPriceCore.Models.OrderStatus;

namespace DynamicPriceCore.MediatR.CustomerEntity.Queries;

public class GetCustomerInfoQueryHandler
	: IRequestHandler<GetCustomerInfoQuery, CustomerInfoViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public GetCustomerInfoQueryHandler(DynamicPriceCoreContext context, IMapper mapper, IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<CustomerInfoViewModel> Handle(GetCustomerInfoQuery request, CancellationToken cancellationToken)
	{
		var customer = await _userService.GetCurrentUserAsync();

		var customerOrders = await _context.Orders
			.Include(o => o.Company)
			.Include(o => o.OrderItems)			// todo: check
				.ThenInclude(oi => oi.Product)
			.Where(o => o.CustomerId == customer.Id
				&& (o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Completed))
			.ToArrayAsync(cancellationToken);

		var ordersVm = _mapper.Map<OrderInfoViewModel[]>(customerOrders);

		var customerInfo = new CustomerInfoViewModel
		{
			Name = customer.UserName,
			Balance = customer.Balance,
			Orders = ordersVm
		};

		return customerInfo;
	}
}
