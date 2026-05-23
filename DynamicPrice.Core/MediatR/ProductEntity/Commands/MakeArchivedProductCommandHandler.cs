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
				p.Id == request.ProductId && 
				p.CompanyId == manager.CompanyId, cancellationToken)
			?? throw new NotFoundException("Product not found.");

		product.UpdateStatus(ProductStatus.Archived);

		await context.SaveChangesAsync(cancellationToken);
	}
}
