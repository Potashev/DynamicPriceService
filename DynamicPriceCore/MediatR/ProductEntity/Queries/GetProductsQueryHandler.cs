using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.ProductEntity.Queries;

public class GetProductsQueryHandler
	: IRequestHandler<GetProductsQuery, IEnumerable<ProductViewModel>>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public GetProductsQueryHandler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task<IEnumerable<ProductViewModel>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
	{
		var company = await _context.CompanyUsers
			.Where(cu => cu.UserId == request.UserId)
			.Select(cu => cu.Company)
			.FirstOrDefaultAsync();

		var products = await _context.Products
			.Where(p => p.Company.CompanyId == company.CompanyId)
			.ToListAsync(cancellationToken);

		return _mapper.Map<List<ProductViewModel>>(products);
	}
}
