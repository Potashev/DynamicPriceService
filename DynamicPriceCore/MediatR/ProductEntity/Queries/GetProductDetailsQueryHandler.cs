using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.ProductEntity.Queries;

public class GetProductDetailsQueryHandler
	: IRequestHandler<GetProductDetailsQuery, Product>
{
	private readonly DynamicPriceCoreContext _context;

	public GetProductDetailsQueryHandler(DynamicPriceCoreContext context)
		=> _context = context;

	public async Task<Product> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken) =>
        await _context.Products.FirstOrDefaultAsync(product => product.ProductId == request.ProductId, cancellationToken);
}
