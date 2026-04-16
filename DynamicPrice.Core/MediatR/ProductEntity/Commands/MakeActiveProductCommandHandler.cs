using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public class MakeActiveProductCommandHandler(
	DynamicPriceCoreContext context,
	IUserService userService)
	: IRequestHandler<MakeActiveProductCommand>
{
	public async Task Handle(
		MakeActiveProductCommand request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var product = await context.Products
			.FirstOrDefaultAsync(p => 
				p.ProductId == request.ProductId && 
				p.CompanyId == manager.CompanyId, cancellationToken)
			?? throw new NotFoundException("Product not found.");

		//context.Products.Remove(product);

		//product.Status = ProductStatus.Archived;

		if (product.Status != ProductStatus.Active)
			product.Status  = ProductStatus.Active;
		else
			throw new BusinessException("Product already active.");

		await context.SaveChangesAsync(cancellationToken);
	}
}
