using AutoMapper;
using DynamicPriceService.Data;
using DynamicPriceService.MediatR.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceService.MediatR.ProductEntity.Queries;

public class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, IEnumerable<ProductViewModel>>
{
    private readonly DynamicPriceServiceContext _context;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(DynamicPriceServiceContext context, IMapper mapper)
        => (_context, _mapper) = (context, mapper);

    public async Task<IEnumerable<ProductViewModel>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _context.Product.ToListAsync(cancellationToken);
        return _mapper.Map<List<ProductViewModel>>(products);
    }
}
