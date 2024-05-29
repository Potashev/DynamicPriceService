using DynamicPriceService.Data;
using DynamicPriceService.Models;
using MediatR;
using Microsoft.CodeAnalysis;

namespace DynamicPriceService.MediatR.ProductEntity.Commands;

public class CreateProductCommandHandler
	: IRequestHandler<CreateProductCommand>
{
	private readonly DynamicPriceServiceContext _context;

	public CreateProductCommandHandler(DynamicPriceServiceContext context)
		=> _context = context;

	public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
	{
		var product = request.Product;
		product.Company = GetCompany();
		product.LastSellTime = DateTime.UtcNow;

		await _context.Product.AddAsync(product, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
	}

	private Company GetCompany() => _context.Company.FirstOrDefault();
}
