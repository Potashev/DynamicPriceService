using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Services;
using DynamicPrice.Core.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public class GetCompanyOrderDetailsQueryHandler
	: IRequestHandler<GetCompanyOrderDetailsQuery, OrderViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public GetCompanyOrderDetailsQueryHandler(DynamicPriceCoreContext context, IMapper mapper, IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<OrderViewModel> Handle(GetCompanyOrderDetailsQuery request, CancellationToken cancellationToken)
	{
		var manager = await _userService.GetCurrentUserAsync();

		var companyOrder = await _context.Orders
			.Where(o => o.OrderId.ToString() == request.OrderId && o.Company.CompanyId == manager.CompanyId) //todo: check and perfomance - convert request to int?
			.Include(o => o.OrderItems)
				.ThenInclude(op => op.Product)
			.FirstOrDefaultAsync(cancellationToken);

		var companyOrderVm = _mapper.Map<OrderViewModel>(companyOrder);

		companyOrderVm.CustomerName = (await _userService.GetUserByIdAsync(companyOrderVm.CustomerId)).UserName;
		companyOrderVm.OrderTotal = GetOrderPrice(companyOrderVm);  //todo: add extension for ordervm or linq?

		return companyOrderVm;
	}

	private decimal GetOrderPrice(OrderViewModel orderVm)
	{
		decimal sum = 0;
		foreach (var orderItem in orderVm.OrderItems)
		{
			sum += (decimal)(orderItem.ProductPrice * orderItem.Quantity);
		}
		return sum;
	}
}
