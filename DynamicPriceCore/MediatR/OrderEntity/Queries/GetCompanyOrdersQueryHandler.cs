using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using DynamicPriceCore.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetCompanyOrdersQueryHandler
	: IRequestHandler<GetCompanyOrdersQuery, IEnumerable<OrderViewModel>>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public GetCompanyOrdersQueryHandler(DynamicPriceCoreContext context, IMapper mapper, IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<IEnumerable<OrderViewModel>> Handle(GetCompanyOrdersQuery request, CancellationToken cancellationToken)
	{
		var manager = await _userService.GetCurrentUserAsync();

		var companyOrders = await _context.Orders
			.Where(o => o.Company.CompanyId == manager.CompanyId)
			.ToArrayAsync(cancellationToken);

		var companyOrdersVm = _mapper.Map<OrderViewModel[]>(companyOrders);

		foreach (var orderVm in companyOrdersVm) 
		{
			orderVm.CustomerName = await _userService.GetUserNameByIdAsync(orderVm.CustomerId);
		}

		return companyOrdersVm;
	}
}
