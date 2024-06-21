using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public class CreateProductCommandHandler
	: IRequestHandler<CreateProductCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public CreateProductCommandHandler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
	{
		var company = await _context.CompanyUsers
			.Where(cu => cu.UserId == request.UserId)
			.Select(cu => cu.Company)
			.FirstOrDefaultAsync();

		var product = _mapper.Map<Product>(request.ProductVm);
		product.Company = company;
		product.LastSellTime = DateTime.UtcNow;

		await _context.Products.AddAsync(product, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		return product.ProductId;
	}
}
