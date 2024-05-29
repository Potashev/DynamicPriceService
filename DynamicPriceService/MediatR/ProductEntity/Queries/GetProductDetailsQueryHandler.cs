using DynamicPriceService.Data;
using DynamicPriceService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceService.MediatR.ProductEntity.Queries;

public class GetProductDetailsQueryHandler
	: IRequestHandler<GetProductDetailsQuery, Product>
{
	private readonly DynamicPriceServiceContext _context;

	public GetProductDetailsQueryHandler(DynamicPriceServiceContext context)
		=> _context = context;

	public async Task<Product> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken) =>
        await _context.Product.FirstOrDefaultAsync(product => product.ProductId == request.ProductId, cancellationToken);
}
