using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetCompanyOrdersQueryHandler
	: IRequestHandler<GetCompanyOrdersQuery, IEnumerable<OrderViewModel>>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly ICurrentUserService _currentUserService;

	public GetCompanyOrdersQueryHandler(DynamicPriceCoreContext context, IMapper mapper, ICurrentUserService currentUserService)
		=> (_context, _mapper, _currentUserService) = (context, mapper, currentUserService);

	public async Task<IEnumerable<OrderViewModel>> Handle(GetCompanyOrdersQuery request, CancellationToken cancellationToken)
	{
		var manager = await _currentUserService.GetCurrentUserAsync();

		//todo: check
		var companyOrders = await _context.Orders
			.Where(o => o.Company.CompanyId == manager.CompanyId)
			.ToArrayAsync(cancellationToken);


		return _mapper.Map<OrderViewModel[]>(companyOrders);
	}
}
