using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public class GetCompanyOrderDetailsQueryHandler
	: IRequestHandler<GetCompanyOrderDetailsQuery, OrderViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public GetCompanyOrderDetailsQueryHandler(
		DynamicPriceCoreContext context,
		IMapper mapper,
		IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<OrderViewModel> Handle(
		GetCompanyOrderDetailsQuery request,
		CancellationToken cancellationToken)
	{
		var manager = await _userService.GetRequiredCurrentUserAsync();

		var companyOrder = await _context.Orders
			.Where(o => o.OrderId.ToString() == request.OrderId && o.CompanyId == manager.CompanyId)
			.Include(o => o.OrderItems)
				.ThenInclude(op => op.Product)
			.FirstOrDefaultAsync(cancellationToken);

		var companyOrderVm = _mapper.Map<OrderViewModel>(companyOrder);

		companyOrderVm.CustomerName = (await _userService.GetUserByIdAsync(companyOrderVm.CustomerId))?.UserName ?? string.Empty;

		return companyOrderVm;
	}
}
