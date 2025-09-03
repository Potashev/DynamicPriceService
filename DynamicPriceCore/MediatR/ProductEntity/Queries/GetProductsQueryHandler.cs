using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using DynamicPriceCore.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.ProductEntity.Queries;

public class GetProductsQueryHandler
	: IRequestHandler<GetProductsQuery, IEnumerable<ProductViewModel>>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public GetProductsQueryHandler(DynamicPriceCoreContext context, IMapper mapper, IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<IEnumerable<ProductViewModel>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
	{
		var manager = await _userService.GetCurrentUserAsync();

		var products = await _context.Products
			.Where(p => p.Company.CompanyId == manager.CompanyId)
			.ToArrayAsync(cancellationToken);

		return _mapper.Map<ProductViewModel[]>(products);
	}
}
