using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public class CreateProductCommandHandler
	: IRequestHandler<CreateProductCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly ICurrentUserService _currentUserService;

	public CreateProductCommandHandler(DynamicPriceCoreContext context, IMapper mapper, ICurrentUserService currentUserService)
		=> (_context, _mapper, _currentUserService) = (context, mapper, currentUserService);

	public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
	{
		var manager = await _currentUserService.GetCurrentUserAsync();

		//var company = await _context.CompanyUsers
		//	.Where(cu => cu.UserId == request.UserId)
		//	.Select(cu => cu.Company)
		//	.FirstOrDefaultAsync(cancellationToken);

		var product = _mapper.Map<Product>(request.ProductVm);
		product.Company = manager.Company;
		product.LastSellTime = DateTime.UtcNow;

		await _context.Products.AddAsync(product, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		return product.ProductId;
	}
}
