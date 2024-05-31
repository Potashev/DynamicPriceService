using DynamicPriceService.Data;
using MediatR;

namespace DynamicPriceService.MediatR.ProductEntity.Commands;

public class DeleteProductCommandHandler
	: IRequestHandler<DeleteProductCommand>
{
	private readonly DynamicPriceServiceContext _context;

	public DeleteProductCommandHandler(DynamicPriceServiceContext context)
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
