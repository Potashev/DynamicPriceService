using DynamicPriceService.Data;
using DynamicPriceService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceService.MediatR.ProductEntity.Queries;

public class GetProductsQueryHandler
	: IRequestHandler<GetProductsQuery, IEnumerable<Product>>
{
	private readonly DynamicPriceServiceContext _context;

	public GetProductsQueryHandler(DynamicPriceServiceContext context)
		=> _context = context;

	public async Task<IEnumerable<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken) =>
		 await _context.Product.ToListAsync(cancellationToken);
}
