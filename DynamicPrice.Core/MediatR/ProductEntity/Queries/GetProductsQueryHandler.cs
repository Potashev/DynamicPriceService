using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.ProductEntity.Queries;

public class GetProductsQueryHandler(
	DynamicPriceCoreContext context,
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<GetProductsQuery, IEnumerable<ProductViewModel>>
{
	public async Task<IEnumerable<ProductViewModel>> Handle(
		GetProductsQuery request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var products = await context.Products
			.Where(p => 
				p.Company.CompanyId == manager.CompanyId &&
				p.Status == ProductStatus.Active)
			.ToArrayAsync(cancellationToken);

		return mapper.Map<ProductViewModel[]>(products);
	}
}
