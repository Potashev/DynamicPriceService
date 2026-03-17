using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.ProductEntity.Queries;

public class GetProductDetailsQueryHandler
	: IRequestHandler<GetProductDetailsQuery, ProductViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public GetProductDetailsQueryHandler(
		DynamicPriceCoreContext context,
		IMapper mapper,
		IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<ProductViewModel> Handle(
		GetProductDetailsQuery request,
		CancellationToken cancellationToken)
	{
		var manager = await _userService.GetRequiredCurrentUserAsync();

		var product = await _context.Products
			.Include(p => p.PriceDynamics)
			.FirstOrDefaultAsync(p => 
				p.ProductId == request.ProductId && 
				p.CompanyId == manager.CompanyId, cancellationToken)
			?? throw new NotFoundException("Product not found.");

		return _mapper.Map<ProductViewModel>(product);
	}
}
