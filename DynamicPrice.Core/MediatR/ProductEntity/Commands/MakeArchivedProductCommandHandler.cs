using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public class MakeArchivedProductCommandHandler(
	DynamicPriceCoreContext context,
	IUserService userService)
	: IRequestHandler<MakeArchivedProductCommand>
{
	public async Task Handle(
		MakeArchivedProductCommand request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var product = await context.Products
			.FirstOrDefaultAsync(p => 
				p.ProductId == request.ProductId && 
				p.CompanyId == manager.CompanyId, cancellationToken)
			?? throw new NotFoundException("Product not found.");

		//todo: add product.UpdateStatus(ProductStatus.Archived)?
		if (product.Status != ProductStatus.Archived)
			product.Status  = ProductStatus.Archived;
		else
			throw new BusinessException("Product already archived.");

		await context.SaveChangesAsync(cancellationToken);
	}
}
