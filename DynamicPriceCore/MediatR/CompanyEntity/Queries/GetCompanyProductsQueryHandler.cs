using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
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
		var products = await _context.Products
			.Where(p => p.Company.CompanyId.ToString() == request.CompanyId)
			.Include(p => p.PriceDynamics)  //todo: set lenght?
			.ToArrayAsync(cancellationToken);

		var productsInfoVm = _mapper.Map<ProductInfoViewModel[]>(products);
		return new CompanyProductsInfo(request.CompanyId, productsInfoVm);
	}
}
