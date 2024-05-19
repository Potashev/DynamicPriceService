using DynamicPriceService.Data;
using DynamicPriceService.Models;
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
        var product = _context.Product.Find(request.ProductId);
        if (product != null)
        {
            _context.Product.Remove(product);
        }

        _context.SaveChanges();
    }
}
