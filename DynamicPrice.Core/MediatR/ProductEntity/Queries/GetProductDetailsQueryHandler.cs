using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.ProductEntity.Queries;

public class GetProductDetailsQueryHandler(
	DynamicPriceCoreContext context,
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<GetProductDetailsQuery, ProductViewModel>
{
	public async Task<ProductViewModel> Handle(
		GetProductDetailsQuery request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var product = await context.Products
			.Include(p => p.PriceDynamics)
			.FirstOrDefaultAsync(p => 
				p.ProductId == request.ProductId && 
				p.CompanyId == manager.CompanyId, cancellationToken)
			?? throw new NotFoundException("Product not found.");

		return mapper.Map<ProductViewModel>(product);
	}
}
