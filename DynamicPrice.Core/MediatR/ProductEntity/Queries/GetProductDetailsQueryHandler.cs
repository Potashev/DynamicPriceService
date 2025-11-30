using AutoMapper;
using DynamicPrice.Core.Data;
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

	public GetProductDetailsQueryHandler(DynamicPriceCoreContext context, IMapper mapper, IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<ProductViewModel> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
	{
		var manager = await _userService.GetCurrentUserAsync();

		var product = await _context.Products
			.Include(p => p.PriceDynamics)
			.FirstOrDefaultAsync(product => product.ProductId == request.ProductId && product.CompanyId == manager.CompanyId, cancellationToken);
		return _mapper.Map<ProductViewModel>(product);
	}
}
