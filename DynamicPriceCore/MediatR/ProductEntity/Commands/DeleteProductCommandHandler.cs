using DynamicPriceCore.Data;
using MediatR;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public class DeleteProductCommandHandler
	: IRequestHandler<DeleteProductCommand>
{
	private readonly DynamicPriceCoreContext _context;

	public DeleteProductCommandHandler(DynamicPriceCoreContext context)
		=> _context = context;

	public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
	{
		//disposed with await
		var product = _context.Products.Find(request.ProductId);
		if (product != null)
		{
			_context.Products.Remove(product);
		}

		_context.SaveChanges();
	}
}
