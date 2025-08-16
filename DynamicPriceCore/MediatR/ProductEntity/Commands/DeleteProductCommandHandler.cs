using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public class DeleteProductCommandHandler
	: IRequestHandler<DeleteProductCommand>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly ICurrentUserService _currentUserService;

	public DeleteProductCommandHandler(DynamicPriceCoreContext context, ICurrentUserService currentUserService)
		=> (_context, _currentUserService) = (context, currentUserService);

	public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
	{
		var manager = await _currentUserService.GetCurrentUserAsync();

		var product = await _context.Products
			.FirstOrDefaultAsync(p =>  p.ProductId == request.ProductId && p.CompanyId == manager.CompanyId, cancellationToken);
		if (product != null)
		{
			_context.Products.Remove(product);
			await _context.SaveChangesAsync(cancellationToken);
		}

	}
}
