using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.CompanyEntity.Queries;

public class GetCompanyProductsQueryHandler
	: IRequestHandler<GetCompanyProductsQuery, CompanyProductsInfo>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public GetCompanyProductsQueryHandler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task<CompanyProductsInfo> Handle(GetCompanyProductsQuery request, CancellationToken cancellationToken)
	{

		var company = await _context.Companies
			.FirstOrDefaultAsync(c => c.CompanyId.ToString() == request.CompanyId, cancellationToken);

		if (company == null)
			throw new KeyNotFoundException($"Company with ID {request.CompanyId} not found.");

		var products = await _context.Products
			.Where(p => p.CompanyId == company.CompanyId)
			.Include(p => p.PriceDynamics)  //todo: set lenght?
			.ToArrayAsync(cancellationToken);

		var companyVm = _mapper.Map<CompanyViewModel>(company);
		var productsInfoVm = _mapper.Map<ProductInfoViewModel[]>(products);
		return new CompanyProductsInfo(companyVm, productsInfoVm);
	}
}
