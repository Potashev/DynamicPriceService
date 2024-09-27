using DynamicPriceCore.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public class DeleteProductCommandHandler
	: IRequestHandler<DeleteProductCommand>
{
	private readonly DynamicPriceCoreContext _context;

	public DeleteProductCommandHandler(DynamicPriceCoreContext context)
		=> _context = context;

	public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
	{
		var product = await _context.Products
			.FirstOrDefaultAsync(p =>  p.ProductId == request.ProductId, cancellationToken);
		if (product != null)
		{
			_context.Products.Remove(product);
			await _context.SaveChangesAsync(cancellationToken);
		}

	}
}
