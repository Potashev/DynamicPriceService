using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetCompanyOrdersQueryHandler
	: IRequestHandler<GetCompanyOrdersQuery, IEnumerable<OrderViewModel>>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public GetCompanyOrdersQueryHandler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task<IEnumerable<OrderViewModel>> Handle(GetCompanyOrdersQuery request, CancellationToken cancellationToken)
	{
		var companyOrders = await _context.Orders
			.Where(o => o.Company.CompanyUsers.Any(cu => cu.UserId == request.UserId))
			.ToArrayAsync(cancellationToken);

		return _mapper.Map<OrderViewModel[]>(companyOrders);
	}
}
