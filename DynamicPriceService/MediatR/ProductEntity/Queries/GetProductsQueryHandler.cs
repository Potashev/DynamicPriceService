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

	public async Task<IEnumerable<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var company = await _context.CompanyUsers
            .Where(cu => cu.UserId == request.UserId)
            .Select(cu => cu.Company)
            .FirstOrDefaultAsync();

        var products = await _context.Products
            .Where(p => p.Company.CompanyId == company.CompanyId)
            .ToListAsync(cancellationToken);
        return products;
    }
}
