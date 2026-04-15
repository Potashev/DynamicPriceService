using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Shared.Contracts.ViewModels;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.CompanyEntity.Queries;

public class GetCompanyProductsQueryHandler(
	DynamicPriceCoreContext context,
	IMapper mapper)
	: IRequestHandler<GetCompanyProductsQuery, CompanyProductsInfo>
{
	public async Task<CompanyProductsInfo> Handle(
		GetCompanyProductsQuery request,
		CancellationToken cancellationToken)
	{
		var company = await context.ActiveCompanies
			.Where(ac => ac.CompanyId.ToString() == request.CompanyId)
			.Select(ac => ac.Company)
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException("Company not found or not active");

		var products = await context.Products
			.Where(p => p.CompanyId == company.CompanyId)
			.Where(Product.CanBeReducedExpr)
			.Include(p => p.PriceDynamics
				.OrderByDescending(pd => pd.Date)
				.Take(company.PriceHistoryLimit))
			.ToArrayAsync(cancellationToken);

		return new CompanyProductsInfo
		{
			Company = mapper.Map<CompanyViewModel>(company),
			Products = mapper.Map<ProductInfoViewModel[]>(products),
			PriceHistoryLimit = company.PriceHistoryLimit
		};
	}
}
