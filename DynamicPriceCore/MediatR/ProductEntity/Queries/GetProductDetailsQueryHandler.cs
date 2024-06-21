using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.ProductEntity.Queries;

public class GetProductDetailsQueryHandler
	: IRequestHandler<GetProductDetailsQuery, ProductViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public GetProductDetailsQueryHandler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task<ProductViewModel> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
	{
		var product = await _context.Products.FirstOrDefaultAsync(product => product.ProductId == request.ProductId, cancellationToken);
		return _mapper.Map<ProductViewModel>(product);
	}
}
