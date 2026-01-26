using DynamicPrice.Core.Data;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public class DeleteProductCommandHandler
	: IRequestHandler<DeleteProductCommand>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IUserService _userService;

	public DeleteProductCommandHandler(
		DynamicPriceCoreContext context,
		IUserService userService)
		=> (_context, _userService) = (context, userService);

	public async Task Handle(
		DeleteProductCommand request,
		CancellationToken cancellationToken)
	{
		var manager = await _userService.GetCurrentUserAsync();

		var product = await _context.Products
			.FirstOrDefaultAsync(p => p.ProductId == request.ProductId && p.CompanyId == manager.CompanyId, cancellationToken);
		if (product != null)
		{
			_context.Products.Remove(product);
			await _context.SaveChangesAsync(cancellationToken);
		}
	}
}
