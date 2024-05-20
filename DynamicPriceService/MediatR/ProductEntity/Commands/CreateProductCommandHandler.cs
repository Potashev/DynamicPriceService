using AutoMapper;
using DynamicPriceService.Data;
using DynamicPriceService.Models;
using MediatR;
using Microsoft.CodeAnalysis;

namespace DynamicPriceService.MediatR.ProductEntity.Commands;

public class CreateProductCommandHandler
	: IRequestHandler<CreateProductCommand>
{
	private readonly DynamicPriceServiceContext _context;
	private readonly IMapper _mapper;

	public CreateProductCommandHandler(DynamicPriceServiceContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
	{
		var product = _mapper.Map<Product>(request.ProductVm);
		product.Company = GetCompany();
		product.LastSellTime = DateTime.UtcNow;

		await _context.Product.AddAsync(product, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
	}

	private Company GetCompany() => _context.Company.FirstOrDefault();
}
