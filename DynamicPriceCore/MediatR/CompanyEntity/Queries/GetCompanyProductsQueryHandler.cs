using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ProductEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.CompanyEntity.Queries;

public class GetCompanyProductsQueryHandler
	: IRequestHandler<GetCompanyProductsQuery, IEnumerable<ProductInfoViewModel>>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public GetCompanyProductsQueryHandler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task<IEnumerable<ProductInfoViewModel>> Handle(GetCompanyProductsQuery request, CancellationToken cancellationToken)
	{
		var products = await _context.Products
			.Where(p => p.Company.CompanyId.ToString() == request.CompanyId)
			.ToListAsync();

		return _mapper.Map<List<ProductInfoViewModel>>(products);
	}
}
