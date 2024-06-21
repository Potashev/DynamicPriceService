using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
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
		var products = await _context.Products
			.Where(p => p.Company.CompanyUsers.Any(cu => cu.UserId == request.UserId))
			.ToListAsync(cancellationToken);

		return _mapper.Map<List<ProductViewModel>>(products);
	}
}
