using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public class DeleteProductCommandHandler(
	DynamicPriceCoreContext context,
	IUserService userService)
	: IRequestHandler<DeleteProductCommand>
{
	public async Task Handle(
		DeleteProductCommand request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var product = await context.Products
			.FirstOrDefaultAsync(p => 
				p.ProductId == request.ProductId && 
				p.CompanyId == manager.CompanyId, cancellationToken)
			?? throw new NotFoundException("Product not found.");

		context.Products.Remove(product);
		await context.SaveChangesAsync(cancellationToken);
	}
}
