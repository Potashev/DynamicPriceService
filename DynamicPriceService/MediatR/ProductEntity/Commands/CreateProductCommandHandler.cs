using DynamicPriceService.Data;
using DynamicPriceService.Models;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceService.MediatR.ProductEntity.Commands;

public class CreateProductCommandHandler
	: IRequestHandler<CreateProductCommand, int>
{
	private readonly DynamicPriceServiceContext _context;

	public CreateProductCommandHandler(DynamicPriceServiceContext context)
		=> _context = context;

	public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
	{
        var company = await _context.CompanyUsers
            .Where(cu => cu.UserId == request.UserId)
            .Select(cu => cu.Company)
            .FirstOrDefaultAsync();

        var product = request.Product;
		product.Company = company;
		product.LastSellTime = DateTime.UtcNow;

		await _context.Products.AddAsync(product, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

        return product.ProductId;
	}
}
