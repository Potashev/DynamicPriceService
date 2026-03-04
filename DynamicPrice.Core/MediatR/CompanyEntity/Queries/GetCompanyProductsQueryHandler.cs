using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Shared.Contracts.ViewModels;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.CompanyEntity.Queries;

public class GetCompanyProductsQueryHandler
	: IRequestHandler<GetCompanyProductsQuery, CompanyProductsInfo>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public GetCompanyProductsQueryHandler(
		DynamicPriceCoreContext context,
		IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task<CompanyProductsInfo> Handle(
		GetCompanyProductsQuery request,
		CancellationToken cancellationToken)
	{

		var company = await _context.Companies
			.FirstOrDefaultAsync(c => c.CompanyId.ToString() == request.CompanyId, cancellationToken)
			?? throw new NotFoundException("Company not found");

		var products = await _context.Products
			.Where(p => p.CompanyId == company.CompanyId)
			.Include(p => p.PriceDynamics
				.OrderByDescending(pd => pd.Date)
				.Take(company.PriceHistoryLimit))	//todo: check
			.ToArrayAsync(cancellationToken);

		return new CompanyProductsInfo
		{
			Company = _mapper.Map<CompanyViewModel>(company),
			Products = _mapper.Map<ProductInfoViewModel[]>(products)
		};
	}
}
