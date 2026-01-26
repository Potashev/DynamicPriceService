using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public class GetCustomerOrderDetailsQueryHandler
	: IRequestHandler<GetCustomerOrderDetailsQuery, OrderViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public GetCustomerOrderDetailsQueryHandler(DynamicPriceCoreContext context, IMapper mapper, IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<OrderViewModel> Handle(GetCustomerOrderDetailsQuery request, CancellationToken cancellationToken)
	{
		var customer = await _userService.GetCurrentUserAsync();

		var customerOrder = await _context.Orders
			.Where(o => o.OrderId.ToString() == request.OrderId && o.CustomerId == customer.Id)
			.Include(o => o.OrderItems)
				.ThenInclude(op => op.Product)
			.Include(o => o.Company)
			.FirstOrDefaultAsync(cancellationToken);

		var customerOrderVm = _mapper.Map<OrderViewModel>(customerOrder);

		return customerOrderVm;
	}
}
