using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public class EditProductCommandHandler(
	DynamicPriceCoreContext context,
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<EditProductCommand, int>
{
	public async Task<int> Handle(
		EditProductCommand request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var updatedProductVm = request.ProductVm;

		var product = await context.Products
			.FirstOrDefaultAsync(p => 
				p.ProductId == updatedProductVm.ProductId && 
				p.CompanyId == manager.CompanyId, cancellationToken)
			?? throw new NotFoundException("Product not found.");

		mapper.Map(updatedProductVm, product);

		context.Update(product);
		await context.SaveChangesAsync(cancellationToken);

		return product.ProductId;
	}
}
