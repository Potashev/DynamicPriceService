using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public class CreateProductCommandHandler(
	DynamicPriceCoreContext context,
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<CreateProductCommand, int>
{
	public async Task<int> Handle(
		CreateProductCommand request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var companyId = manager.CompanyId
			?? throw new BusinessException("У пользователя не указан CompanyId.");

		var product = mapper.Map<Product>(request.ProductVm);

		product.CompanyId = companyId;
		product.LastSellTime = DateTime.UtcNow;
		product.Status = ProductStatus.Active;

		await context.Products.AddAsync(product, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);

		return product.ProductId;
	}
}
