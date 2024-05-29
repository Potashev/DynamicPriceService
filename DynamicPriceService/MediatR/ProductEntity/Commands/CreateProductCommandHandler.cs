using DynamicPriceService.Data;
using DynamicPriceService.Models;
using MediatR;
using Microsoft.CodeAnalysis;

namespace DynamicPriceService.MediatR.ProductEntity.Commands;

public class CreateProductCommandHandler
	: IRequestHandler<CreateProductCommand, int>
{
	private readonly DynamicPriceServiceContext _context;

	public CreateProductCommandHandler(DynamicPriceServiceContext context)
		=> _context = context;

	public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
	{
		var product = request.Product;
		product.Company = GetCompany();
		product.LastSellTime = DateTime.UtcNow;

		await _context.Product.AddAsync(product, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

        return product.ProductId;
	}

	private Company GetCompany() => _context.Company.FirstOrDefault();
}
